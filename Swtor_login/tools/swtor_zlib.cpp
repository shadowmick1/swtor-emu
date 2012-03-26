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

#include "swtor_zlib.h"

swzlib::swzlib()
{
	mInboundStream.zalloc = Z_NULL;
	mInboundStream.zfree = Z_NULL;
	mInboundStream.opaque = Z_NULL;
	inflateInit2_(&mInboundStream, -8, "1.2.3", sizeof(mInboundStream));

	mOutboundStream.zalloc = Z_NULL;
	mOutboundStream.zfree = Z_NULL;
	mOutboundStream.opaque = Z_NULL;
	deflateInit2_(&mOutboundStream, Z_DEFAULT_COMPRESSION, Z_DEFLATED, -8, 8, Z_DEFAULT_STRATEGY, "1.2.3", sizeof(mOutboundStream));
}

int swzlib::swtor_decompress(Bytef *source, Bytef *dest, int sizeIn, int sizeOut)
{
	int ret;
	unsigned have;
	int total = 0;

	Bytef* inbound = new Bytef[sizeIn - 2];
	memcpy(inbound, source+6, sizeIn-6);
	inbound[sizeIn - 3] = 0xFF;
	inbound[sizeIn - 4] = 0xFF;
	inbound[sizeIn - 5] = 0x00;
	inbound[sizeIn - 6] = 0x00;

	mInboundStream.avail_in = sizeIn-2;
	if (mInboundStream.avail_in == 0)
		return 0;
	mInboundStream.next_in = inbound;

	do {
		mInboundStream.avail_out = sizeOut;
		mInboundStream.next_out = dest;
		ret = inflate(&mInboundStream, Z_FULL_FLUSH);
		have = sizeOut - mInboundStream.avail_out;
		total += have;

		if(ret != 0)
			printf(mInboundStream.msg);

		printf("ret = %d\n", ret);
		switch (ret) {
		case Z_NEED_DICT:
		case Z_DATA_ERROR:
		case Z_MEM_ERROR:
			delete[] inbound;
			return 0;
		}
	} while (mInboundStream.avail_out == 0);

	delete[] inbound;

	return total;
}

int swzlib::swtor_compress(Byte* inbuf, int insize, Byte* outbuf, int outsize)
{
/*	mOutboundStream.avail_in = insize;
	mOutboundStream.next_in = inbuf;
	mOutboundStream.avail_out = outsize;
	mOutboundStream.next_out = outbuf;

	int ret = deflate(&mOutboundStream, Z_FULL_FLUSH);
	*/
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
	return outsize - zs.avail_out;
}