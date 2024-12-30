#include <iostream>
#include <Windows.h>
#include <time.h>

constexpr int ARRAY_LENGTH = 25;

int main()
{
	HANDLE hDataFile = CreateFileW(L"data.dat", GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, nullptr, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL,nullptr);
	if (hDataFile == NULL) {
		printf("Failed Unable to create data file");
		return 1;
	}

	HANDLE hDataMutex = CreateMutexW(nullptr, true, L"MyDataMutex");
	if (hDataMutex == NULL) {
		printf("Failed to create mutex");
		CloseHandle(hDataFile);
		return 1;
	}

	HANDLE hDataMapping = CreateFileMappingW(hDataFile, nullptr, PAGE_READWRITE, 0, ARRAY_LENGTH, L"MyDataMapping");
	if (hDataMapping == NULL) {
		printf("Failed to create file mapping");
		CloseHandle(hDataFile);
		CloseHandle(hDataMutex);
		return 1;
	}

	LPBYTE arrayData = (LPBYTE)MapViewOfFile(hDataMapping, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, ARRAY_LENGTH);
	if (arrayData == NULL) {
		printf("Failed to map view of file");
		CloseHandle(hDataFile);
		CloseHandle(hDataMutex);
		CloseHandle(hDataMapping);
		return 1;
	}
	srand(time(NULL));
	BYTE data[ARRAY_LENGTH];
	for (int i = 0; i < ARRAY_LENGTH; i++) {
		data[i] = rand() % 91 + 10;
	}
	
	memcpy(arrayData, data, ARRAY_LENGTH);
	printf("Array with random numbers written to file:");
	for (int i = 0; i < ARRAY_LENGTH; i++) {
		printf("%d ", data[i]);
	}
	ReleaseMutex(hDataMutex);
	getchar();
	CloseHandle(hDataMapping);
	CloseHandle(hDataFile);
	CloseHandle(hDataMutex);
	return 0;
}

