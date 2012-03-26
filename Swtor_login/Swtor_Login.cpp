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

#include "Swtor_login.h"
#include "tools/Swtor_GetKeys.h"
#include "tools/Swtor_Salsa.h"
#include "tools/swtor_zlib.h"
#include "tools/swtor_RSA.h"

//External define
extern CGameConnect* gameConnect;
swGetKeys* sGetKeys;
swsalsa*  sSalsa;

static CryptoPP::Salsa20::Decryption* g_ppSalsaDecryptor3;
static CryptoPP::Salsa20::Decryption* g_ppSalsaDecryptor4;

using namespace std;

void PrintBinary(BYTE* data, int length)
{
	for (int i = 0; i < length; i++)
	{
		printf("%02X ", data[i]);
		if((i + 1) % 16 == 0)
			printf("\n");
	}
	printf("\n");
}

DWORD WINAPI Login_Server(LPVOID lpParam)
{
	SOCKET servSock;                 /* Socket descriptor for server */
	SOCKET clnts;                    /* Socket descriptor for client */
	int len;
	struct sockaddr_in echoServAddr; /* Local address */
	struct sockaddr_in echoClntAddr; /* Client address */
	int clntLen;					 /* Length of client address data structure */
	WSADATA wsaData;                 /* Structure for WinSock setup communication */

	if (WSAStartup(MAKEWORD(2, 0), &wsaData) != 0) /* Load Winsock 2.0 DLL */
	{
		printf("SOCKET_SERVER: WSAStartup failed\n");
		return 0;
	}

	/* Create socket for incoming connections */
	servSock = socket(PF_INET, SOCK_STREAM, IPPROTO_TCP);
	if(servSock == INVALID_SOCKET)
	{
		printf("SOCKET_SERVER: Socket failed\n");
		return 0;
	}

	/* Construct local address structure */
	memset(&echoServAddr, 0, sizeof(echoServAddr));   /* Zero out structure */
	echoServAddr.sin_family = AF_INET;                /* Internet address family */

#ifdef CONFIG_SOCKET_SERVER_ADDR
	echoServAddr.sin_addr.s_addr = inet_addr(CONFIG_SOCKET_ADDR); /* Specific incoming interface */
#else
	echoServAddr.sin_addr.s_addr = htonl(INADDR_ANY); /* Any incoming interface */
#endif

	echoServAddr.sin_port = htons(CONFIG_SOCKET_SERVER_PORT);      /* Local port */

	/* Bind to the local address */
	if(bind(servSock, (struct sockaddr *) &echoServAddr, sizeof(echoServAddr)) !=0 )
	{
		printf("SOCKET_SERVER: Bind failed\n", exit);
		return 0;
	}
	/* Mark the socket so it will listen for incoming connections */
	if(listen(servSock,5) != 0)
	{
		printf("SOCKET_SERVER: Listen failed\n", exit);
		return 0;
	}

	//handle_connections(sock, servSock, NULL);
	printf("SOCKET_SERVER: Swtor Login server listening v%s\n", CONFIG_THEATER_VERSION);

	while(1)
	{
		/* Set the size of the in-out parameter */
		clntLen = sizeof(echoClntAddr);
		len = sizeof(struct sockaddr_in);

		/* Wait for a client to connect */
		if((clnts = accept(servSock, (struct sockaddr *) &echoClntAddr, &clntLen)) < 0)
			printf("SOCKET_CLIENT: Accept failed\n", exit);

		//Create connection information
		CConnection clientConnection;
		clientConnection.clnts			= clnts;
		clientConnection.ip				= inet_ntoa(echoClntAddr.sin_addr);
		clientConnection.port			= ntohs(echoClntAddr.sin_port);

		printf("SOCKET_CLIENT: Client Connect To Login Server (%s:%hu)\n", inet_ntoa(echoClntAddr.sin_addr), ntohs(echoClntAddr.sin_port));

		CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)server_server, &clientConnection, 0, 0);
	}

	close(servSock);
	return(0);
}

bool getkeys()
{
	HANDLE hKeysFile = CreateFile("Keys//domo//login_server_keys.dat",
		GENERIC_READ | GENERIC_WRITE,
		FILE_SHARE_READ,
		0,
		CREATE_ALWAYS,
		FILE_ATTRIBUTE_NORMAL,
		0);

	if (INVALID_HANDLE_VALUE == hKeysFile)
	{
		printf("ERROR: can't create file for keys\n");
		return 1;
	}

	while (true)
	{
		if(sGetKeys->GetSWTORKeys(hKeysFile, 0x00))
		{
			CloseHandle(hKeysFile);
			break;
		}
	}
	printf("got key for %s \n","Domo");
}

DWORD WINAPI server_server(LPVOID lpParameter)
{
	swzlib szlib;
	CConnection *connection	= (CConnection *)lpParameter;

	SOCKET clntSock = connection->clnts;

	static const char rawData2[22] =
	{
		0x03, 0x16, 0x00, 0x00, 0x00, 0x15, 0x11, 0x00,
		0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x36, 0x40,
		0xce, 0x72, 0x71, 0xfe, 0x8d, 0x7e
	};

	send(clntSock, rawData2, 22, 0);

	byte SalsaEnc[32] = {0};
	byte SalsaEncSec[8] = {0};
	byte SalsaDec[32] = {0};
	byte SalsaDecSec[8] = {0};

	int i = 0;
	byte data[1024 * 10] = {0};

	for(;;)
	{
		int r = recv( clntSock, (char*)data, 1024 * 10, 0 );
		if(r > 1)
		{
			i++;
			printf("i = #%i\n", i);
			printf("p-l = #%i\n", r);
			//PrintBinary(data , r);
			if( data[0] == '\x04' && data[1] == '\x0A')
			{
				TOR_RSA_Init();

				unsigned int len = 0;
				unsigned char buff[256];

				for(auto k = 0; k < 256; ++k)
					sscanf(((char*)data+10+(k*2)),"%02X",buff+k);

				auto salsaKeys = TOR_RSA_Decrypt(buff, 256,&len);

				printf("%d", len);
				PrintBinary(salsaKeys, len);
				getkeys();
				sGetKeys->init("Keys//domo//login_server_keys.dat", SalsaDec, SalsaDecSec, SalsaEnc, SalsaEncSec);
				g_ppSalsaDecryptor3 = sSalsa->Decrypt(SalsaDec, SalsaDecSec);
				g_ppSalsaDecryptor4 = sSalsa->Encrypt(SalsaEnc, SalsaEncSec);

				static const unsigned char rawData2[66] =
				{
					0xAF, 0xC5, 0x31, 0x67, 0xFF, 0xFF, 0xFF, 0xFF, 0x04, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x63, 0x61,
					0x73, 0x74, 0x6C, 0x65, 0x68, 0x69, 0x6C, 0x6C, 0x74, 0x65, 0x73, 0x74, 0x00, 0x09, 0x00, 0x00,
					0x00, 0x61, 0x66, 0x63, 0x31, 0x62, 0x62, 0x35, 0x61, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x6C, 0x6F,
					0x67, 0x69, 0x6E, 0x73, 0x65, 0x72, 0x76, 0x65, 0x72, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00,
					0x00, 0x00,
				};

				static const byte rawh[6] =
				{
					0x00, 0x42, 0x00, 0x00, 0x00, 0x42
				};

				byte totalbuf[72];

				memset( totalbuf, 0, 72 );
				memcpy( totalbuf, rawh, 6 );
				memcpy( &totalbuf[6], rawData2, 66 );

				byte output[100];
				memset(output, 0, 100);

				int osize = szlib.swtor_compress(totalbuf, 72, output, 100);

				sSalsa->DecryptBuf(output, osize, g_ppSalsaDecryptor4);

				send(clntSock, (char*)output, osize, 0);

				printf("sending #1\n");
				printf("----------------------------------------------------------\n");
			}

			if( (byte)data[0] != '\x04' && (byte)data[1] != '\x0A')
			{
				sSalsa->DecryptBuf(data, r, g_ppSalsaDecryptor3);

				byte* output2 = new byte[2000];
				memset(output2, 0, 2000);

				printf("decompressBuf\n");
				int ren = data[1];

				ren = szlib.swtor_decompress(data, output2, r, 2000 );

				PrintBinary(output2, ren);

				delete[] output2;

				printf("recving #1\n");
				printf("----------------------------------------------------------\n");
			}

			if(i == 2)
			{
				unsigned char rawData2[88] =
				{
					0x84, 0xD0, 0xF2, 0x90, 0x04, 0x00, 0x00, 0x00, 0x1F, 0x00, 0x00, 0x00,
					0x76, 0x61, 0x31, 0x70, 0x6F, 0x6F, 0x6C, 0x64, 0x31, 0x6E, 0x30, 0x30,
					0x35, 0x39, 0x2E, 0x73, 0x77, 0x74, 0x6F, 0x72, 0x2E, 0x63, 0x6F, 0x6D,
					0x3A, 0x32, 0x30, 0x30, 0x36, 0x33, 0x00, 0x29, 0x00, 0x00, 0x00, 0x42,
					0x54, 0x51, 0x4A, 0x43, 0x4B, 0x53, 0x46, 0x49, 0x47, 0x56, 0x53, 0x4A,
					0x56, 0x53, 0x47, 0x56, 0x43, 0x4B, 0x44, 0x4B, 0x54, 0x4B, 0x44, 0x59,
					0x46, 0x47, 0x50, 0x53, 0x43, 0x4C, 0x56, 0x56, 0x44, 0x47, 0x5A, 0x51,
					0x41, 0x47, 0x59, 0x00
				};

				byte output[2000];
				memset( output, 0, 2000 );

				int osize = szlib.swtor_compress( &output[6], 88, output + 6, 1994);

				output[1] = output[5] = osize+2;

				PrintBinary(output, osize+2);

				sSalsa->DecryptBuf(output, osize+6, g_ppSalsaDecryptor4);

				send(clntSock, (char*)output, osize+2, 0);

				printf("sending #2\n");
				printf("----------------------------------------------------------\n");
			}
		}
	}
	return(0);
}