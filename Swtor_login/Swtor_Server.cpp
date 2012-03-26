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
#include "Swtor_Server.h"

int main(int argc, char *argv[])
{
	DWORD LoginThread;
	HANDLE hThreads[1];

	printf("----------------------------\n");
	printf("Swtor Emu Server V%s\n", CONFIG_THEATER_VERSION);
	printf("----------------------------\n\n");

	hThreads[0]	= CreateThread(NULL, 0, Login_Server, NULL, 0, &LoginThread);

	WaitForMultipleObjects(1, hThreads, true, INFINITE);

	return 0;
}