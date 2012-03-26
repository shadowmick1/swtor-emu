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

#include "Swtor_Salsa.h"

static CryptoPP::Salsa20::Decryption* g_ppSalsaDecryptor3;
static CryptoPP::Salsa20::Decryption* g_ppSalsaDecryptor4;

CryptoPP::Salsa20::Decryption* swsalsa::Decrypt(Byte* SalsaDec, Byte* SalsaDecSec)
{
	return g_ppSalsaDecryptor3 = new CryptoPP::Salsa20::Decryption(SalsaDec, 0x20, SalsaDecSec);
}

CryptoPP::Salsa20::Encryption* swsalsa::Encrypt(Byte* SalsaEnc, Byte* SalsaEncSec)
{
	return g_ppSalsaDecryptor4 = new CryptoPP::Salsa20::Decryption(SalsaEnc, 0x20, SalsaEncSec);
}

bool swsalsa::DecryptBuf(BYTE* buf, DWORD dwSize, CryptoPP::Salsa20::Decryption* pSalsaDecryptor)
{
	if (!pSalsaDecryptor)
		return false;

	BYTE* decrypted = new BYTE[dwSize];

	pSalsaDecryptor->ProcessData(decrypted, buf, dwSize);
	memcpy(buf, decrypted, dwSize);

	delete[] decrypted;

	return true;
}