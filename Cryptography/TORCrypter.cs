/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Cryptography
{
	public class TORCrypter
	{
		[DllImport("ToroidEngDll.dll", EntryPoint = "#3")]
		public static extern IntPtr Initialize();
		[DllImport("ToroidEngDll.dll", EntryPoint = "#6")]
		public static extern bool SetDecryptKeys(IntPtr context, byte[] key, byte[] iv);
		[DllImport("ToroidEngDll.dll", EntryPoint = "#7")]
		public static extern bool SetEncryptKeys(IntPtr context, byte[] key, byte[] iv);
		[DllImport("ToroidEngDll.dll", EntryPoint = "#4")]
		public static extern bool PutRaw(IntPtr context, byte[] data, uint length);
		//////////////////////////////////////////////////////////////////////////
		// RETURN: -1 - need more raw data
		//         -2 - invalid header
		//         -3 - compression/decompression error
		//         <0 - error
		//         =0 - ok
		//         >0 - need more buffer space
		[DllImport("ToroidEngDll.dll", EntryPoint = "#1")]
		public static extern int GetPacket(IntPtr context, byte[] destinationBuffer, uint dstLength);
		//////////////////////////////////////////////////////////////////////////
		//// RETURN: 0 - success
		////         -1 - need more Frame data
		////         >0 - need more space
		////         <0 - error
		[DllImport("ToroidEngDll.dll", EntryPoint = "#2")]
		public static extern int GetRaw(IntPtr context, byte[] pFrame, uint nFrameSize, byte[] pRawBuffer, uint nDstAllocatedSize);

		[DllImport("swtor_getkeys.dll", EntryPoint = "GetSWTORKeys", SetLastError = true)]
		public static extern bool GetSWTORKeys(IntPtr hKeysFile, uint dwOffset);
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

		bool isForProtocolWalker;
		/// <summary>
		/// Returns whether the current Crypter is tied for use in Protocol Walker (analysis tool).
		/// </summary>
		public bool IsForProtocolWalker
		{
			get { return isForProtocolWalker; }
		}

		IntPtr rawContextPtr;
		/// <summary>
		/// Returns a pointer to the underlying C++ RawContext.
		/// </summary>
		public IntPtr RawContext
		{
			get { return rawContextPtr; }
		}

		object cryptOpsMutex = new object();
		bool serverCryptInitialized = false;

        byte _keyType;

		public TORCrypter(bool forProtocolWalker, byte type)
		{
			isForProtocolWalker = forProtocolWalker;
			Console.Write("Initializing Raw Data ...");
			rawContextPtr = Initialize();
            _keyType = type;
			Console.WriteLine("done.");
		}

		byte[] serverKey = new byte[32];
		byte[] serverIV = new byte[8];
		byte[] clientKey = new byte[32];
		byte[] clientIV = new byte[8];

		public void LoadKeys(byte type)
		{
			string filename = "c:\\swtor." + DateTime.Now.ToFileTime() + ".salsa20";
			IntPtr keyFile = FileInteropFunctions.CreateFile(filename, FileInteropFunctions.GENERIC_WRITE, 0, IntPtr.Zero, FileInteropFunctions.CREATE_ALWAYS, 0, 0);
			while (true)
			{
				if (GetSWTORKeys(keyFile, type))
				{
					FileInteropFunctions.CloseHandle(keyFile);
					break;
				}
			}
			uint dwRead = 0;
			byte[] dwTampax = new byte[4];
			IntPtr file = FileInteropFunctions.CreateFile(filename, FileInteropFunctions.GENERIC_READ, 0, IntPtr.Zero, FileInteropFunctions.OPEN_EXISTING, 0x80, 0);

			ReadFile(file, dwTampax, 4, out dwRead, IntPtr.Zero);
			ReadFile(file, clientKey, 32, out dwRead, IntPtr.Zero);
			ReadFile(file, clientIV, 8, out dwRead, IntPtr.Zero);
			ReadFile(file, dwTampax, 4, out dwRead, IntPtr.Zero);
			ReadFile(file, dwTampax, 4, out dwRead, IntPtr.Zero);
			
			ReadFile(file, dwTampax, 4, out dwRead, IntPtr.Zero);
			ReadFile(file, serverKey, 32, out dwRead, IntPtr.Zero);
			ReadFile(file, serverIV, 8, out dwRead, IntPtr.Zero);
			ReadFile(file, dwTampax, 4, out dwRead, IntPtr.Zero);

			if (!ReadFile(file, dwTampax, 4, out dwRead, IntPtr.Zero))
			{
				Console.WriteLine("ERROR: Invalid keys file.");
			}
			else
			{
				Console.WriteLine("Loaded Salsa20 Keys.");
			}
		}

		public void InitializeServerCrypt()
		{
			SetEncryptKeys(rawContextPtr, clientKey, clientIV);
			SetDecryptKeys(rawContextPtr, serverKey, serverIV);
			serverCryptInitialized = true;
		}

		public void InitializeWalker(bool server)
		{
			if (server)
				SetDecryptKeys(rawContextPtr, clientKey, clientIV);
			else
				SetDecryptKeys(rawContextPtr, serverKey, serverIV);
		}

		public List<byte[]> ProcessWalker(byte[] data)
		{
			lock (cryptOpsMutex)
			{
				PutRaw(rawContextPtr, data, (uint)data.Length);
				List<byte[]> packets = new List<byte[]>();
				int packetSize = 0;
				while (true)
				{
					packetSize = GetPacket(rawContextPtr, null, 0);

					if (packetSize == -1) break;

					if (packetSize < 0 && packetSize != -1)
					{
						Console.WriteLine("ERROR: GetPacket() returned " + packetSize);
						break;
					}

					if (packetSize == 0)
						break;

					byte[] packet = new byte[packetSize];
					int res = GetPacket(rawContextPtr, packet, (uint)packetSize);
					if (res == 0)
					{
						packets.Add(packet);
					}
				}
				return packets;
			}
		}

		public byte[] ProcessServerToClient(byte[] data)
		{
			lock (cryptOpsMutex)
			{
				byte[] compressed = new byte[data.Length];
				uint compressedSize = (uint)compressed.Length;
				int newSize = GetRaw(rawContextPtr, data, (uint)data.Length, null, (uint)0);
				Console.WriteLine("Compression : " + data.Length + " => " + newSize);
				return new byte[0];
			}
		}

		public List<byte[]> ProcessServerFromClient(byte[] data)
		{
			lock (cryptOpsMutex)
			{
				List<byte[]> packets = new List<byte[]>();
				PutRaw(rawContextPtr, data, (uint)data.Length);
				int packetSize = 0;
				while (true)
				{
					packetSize = GetPacket(rawContextPtr, null, 0);

					if (packetSize == -1) break;

					if (packetSize < 0)
					{
						Console.WriteLine("ERROR: GetPacket() returned " + packetSize);
						break;
					}

					byte[] packet = new byte[packetSize];
					int res = GetPacket(rawContextPtr, packet, (uint)packetSize);
					if (res == 0)
					{
						Console.WriteLine("RecvPacket => " + packetSize);
						packets.Add(packet);
					}
				}
				return packets;
			}
		}
	}

	class FileInteropFunctions
	{
		public const uint GENERIC_READ = (0x80000000);
		public const uint GENERIC_WRITE = (0x40000000);
		public const uint GENERIC_EXECUTE = (0x20000000);
		public const uint GENERIC_ALL = (0x10000000);

		public const uint CREATE_NEW = 1;
		public const uint CREATE_ALWAYS = 2;
		public const uint OPEN_EXISTING = 3;
		public const uint OPEN_ALWAYS = 4;
		public const uint TRUNCATE_EXISTING = 5;

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateFile(
			String lpFileName,              // Filename
			uint dwDesiredAccess,              // Access mode
			uint dwShareMode,              // Share mode
			IntPtr attr,                   // Security Descriptor
			uint dwCreationDisposition,           // How to create
			uint dwFlagsAndAttributes,           // File attributes
			uint hTemplateFile);               // Handle to template file

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool FlushFileBuffers(IntPtr hFile);
	}
}
