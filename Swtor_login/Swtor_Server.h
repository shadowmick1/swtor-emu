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

#pragma once
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <ctype.h>
#include <time.h>
#include <sys/stat.h>
#include <iostream>
#include <vector>
#include <fstream>
#include <io.h>
#include <fcntl.h>
#include <commctrl.h>
#include <vector>
#include <iostream>
#include <fstream>

#include <salsa.h>
#pragma comment(lib, "cryptlib.lib")

#include <zlib.h>
#pragma comment(lib, "zdll.lib")

//Include socket
#include <winsock.h>
#pragma comment(lib, "wsock32.lib")

#define close   closesocket
#define sleep   Sleep
#define ONESEC  1000

//Structures
//Type defines
typedef unsigned int	uint;
typedef uint8_t     u8;
typedef uint16_t    u16;
typedef uint32_t    u32;

struct CConnection
{
	SOCKET	clnts;
	char*	ip;
	uint	port;
};

struct CGameConnect
{
	int status;
	u8* GID;
	u8* clientPort;
	char* clientIp;
	u8* clientIntPort;
	u8* clientIntIp;
	char* clientName;
	char* UID;
	int ticket;
};

/*----------------*/
/*-----Config-----*/

#define CONFIG_THEATER_VERSION			"1.3"

//#define CONFIG_SOCKET_SERVER_ADDR		    "127.0.0.1"
#define CONFIG_SOCKET_SERVER_PORT		    7979

//#define CONFIG_SOCKET_SERVER_ADDR		    "127.0.0.1"
#define CONFIG_SOCKET_GAME_PORT				20065

#define CONFIG_MYSQL_HOST				"127.0.0.1"
#define CONFIG_MYSQL_USERNAME			"ea_fesl"
#define CONFIG_MYSQL_PASSWORD			"*******"
#define CONFIG_MYSQL_DATABASE			"ea_fesl"
#define CONFIG_MYSQL_PORT				3306
/*-----Config-----*/
/*----------------*/