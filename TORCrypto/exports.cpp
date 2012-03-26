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

#include "tor-rsa.h"
#include "tor-zlib.h"
#include "cryptopp/include/salsa.h"

struct TCryptoInteropWrapper
{
	CryptoPP::Salsa20::Decryption* decrypter;
	// For Server : CryptoPP::Salsa20::Encryption*
	// For Protocol Walker : CryptoPP::Salsa20::Decryption*
	void* encrypter;
	bool type; // True = Server | False = Protocol Walker
	TOR_Zlib* zlib;
};

void hexdump(void *ptr, int buflen) {
  unsigned char *buf = (unsigned char*)ptr;
  int i, j;
  for (i=0; i<buflen; i+=16) {
    printf("%06x: ", i);
    for (j=0; j<16; j++) 
      if (i+j < buflen)
        printf("%02x ", buf[i+j]);
      else
        printf("   ");
    printf(" ");
    for (j=0; j<16; j++) 
      if (i+j < buflen)
        printf("%c", isprint(buf[i+j]) ? buf[i+j] : '.');
    printf("\n");
  }
}

extern "C"
{
	// Create an interop wrapper
	__declspec(dllexport) TCryptoInteropWrapper* __stdcall TCryptoInteropWrapperCreate(bool type);
	// Load the Salsa keys in this wrapper
	__declspec(dllexport) void __stdcall TCryptoInteropWrapperLoadKeys(TCryptoInteropWrapper* wrapper, unsigned char* key1, unsigned char* iv1, unsigned char* key2, unsigned char* iv2);

	// Initialize the RSA algorithm
	__declspec(dllexport) void __stdcall TRSAInit();
	// Decrypt a chunk of data using RSA
	__declspec(dllexport) unsigned char* __stdcall TRSADecrypt(unsigned char* buffer, unsigned int buffer_len, unsigned int* decrypted_len);

	// Process client data
	__declspec(dllexport) int __stdcall TCryptoProcessClient(TCryptoInteropWrapper* wrapper, unsigned char* data, unsigned int data_len, unsigned char* buffer);
	// Process server data
	__declspec(dllexport) int __stdcall TCryptoProcessServer(TCryptoInteropWrapper* wrapper, unsigned char* data, unsigned int data_len, unsigned char* buffer);
}

TCryptoInteropWrapper* __stdcall TCryptoInteropWrapperCreate(bool type)
{
	TCryptoInteropWrapper* wrapper = new TCryptoInteropWrapper();
	wrapper->type = type;
	wrapper->zlib = new TOR_Zlib();
	return wrapper;
}

void __stdcall TCryptoInteropWrapperLoadKeys(TCryptoInteropWrapper* wrapper, unsigned char* key1, unsigned char* iv1, unsigned char* key2, unsigned char* iv2)
{
	wrapper->decrypter = new CryptoPP::Salsa20::Decryption(key1, 32, iv1);
	if(wrapper->type)
		wrapper->encrypter = new CryptoPP::Salsa20::Encryption(key2, 32, iv2);
	else
		wrapper->encrypter = new CryptoPP::Salsa20::Decryption(key2, 32, iv2);
}

void __stdcall TRSAInit()
{
	TOR_RSA_Init();
}

unsigned char* __stdcall TRSADecrypt(unsigned char* buffer, unsigned int buffer_len, unsigned int* decrypted_len)
{
	return TOR_RSA_Decrypt(buffer, buffer_len, decrypted_len);
}

int __stdcall TCryptoProcessClient(TCryptoInteropWrapper* wrapper, unsigned char* data, unsigned int data_len, unsigned char* buffer)
{
	unsigned char* tmp = (unsigned char*)malloc(data_len);
	memset(tmp, 0, data_len);
	// salsa decrypt
	wrapper->decrypter->ProcessData(tmp, data, data_len);	
	// zlib decompress
	int size = data_len * 2;
	wrapper->zlib->swtor_zInit();
	size = wrapper->zlib->swtor_decompress(tmp, buffer + 6, data_len, size);
	memcpy(buffer, tmp, 6);
	free(tmp);
	return size + 6;
}

int __stdcall TCryptoProcessServer(TCryptoInteropWrapper* wrapper, unsigned char* data, unsigned int data_len, unsigned char* buffer)
{
	unsigned char* compressed = (unsigned char*)malloc(data_len);
	memset(compressed, 0, data_len);
	int size = data_len;
	wrapper->zlib->swtor_zInit();
	size = wrapper->zlib->swtor_compress(data + 6, data_len - 6, compressed, size);

	unsigned char* final = (unsigned char*)malloc(size + 6);
	memcpy(final, data, 6);
	memcpy(final + 6, compressed, size);

	printf("after compression:\n");
	hexdump(final, size + 6);
	((CryptoPP::Salsa20::Encryption*)wrapper->encrypter)->ProcessData(buffer, final, size + 6);
	free(final);
	free(compressed);
	return size + 6;
}