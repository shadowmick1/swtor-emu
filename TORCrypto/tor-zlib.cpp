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

#include "tor-zlib.h"

z_stream strm;

void TOR_Zlib::swtor_zInit()
{
	strm.zalloc = Z_NULL;
	strm.zfree = Z_NULL;
	strm.opaque = Z_NULL;
	strm.avail_in = 0;
	strm.next_in = Z_NULL;
	inflateInit2_(&strm, -8, "1.2.3", sizeof(strm));
}

int TOR_Zlib::swtor_decompress(Bytef *source, Bytef *dest, int sizeIn, int sizeOut)
{
	int ret;
	unsigned have;
	int total = 0;

	/* decompress until deflate stream ends or end of file */
	do {
		strm.avail_in = sizeIn-6;
		if (strm.avail_in == 0)
			break;
		strm.next_in = source+6;

		/* run inflate() on input until output buffer not full */
		do {
			strm.avail_out = sizeOut;
			strm.next_out = dest;
			ret = inflate(&strm, Z_NO_FLUSH);
			have = sizeOut - strm.avail_out;
			total += have;

			switch (ret) {
			case Z_NEED_DICT:
			case Z_DATA_ERROR:
			case Z_MEM_ERROR:
				return total;
			}
		} while (strm.avail_out == 0);

		/* done when inflate() says it's done */
	} while (ret != Z_STREAM_END);

	return total;
}

int TOR_Zlib::swtor_compress(Byte* inbuf, int insize, Byte* outbuf, int outsize)
{
	z_stream zs;
	int result;

	zs.zalloc = Z_NULL;
	zs.zfree = Z_NULL;
	zs.opaque = Z_NULL;
	result = deflateInit2_(&zs, Z_DEFAULT_COMPRESSION, Z_DEFLATED, -8, 8, Z_DEFAULT_STRATEGY, "1.2.3", sizeof(zs));

	zs.avail_in = insize;
	zs.next_in = inbuf;
	zs.avail_out = outsize;
	zs.next_out = outbuf;

	deflate(&zs, Z_SYNC_FLUSH);
	deflateEnd(&zs);
	return (outsize - zs.avail_out);
}