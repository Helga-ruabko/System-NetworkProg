#include <iostream>
#include <Windows.h>

constexpr int ARRAY_LENGTH = 25;

int main()
{
    HANDLE hDataMapping = OpenFileMappingW(FILE_MAP_READ | FILE_MAP_WRITE, false, L"MyDataMapping");
    if (hDataMapping == NULL) {
        printf("Failed to open file mapping");
        return 1;
    }

    HANDLE hDataMutex = OpenMutexW(SYNCHRONIZE, false, L"MyDataMutex");
    if (hDataMutex == NULL) {
        printf("Failed to open mutex");
        CloseHandle(hDataMapping);
        return 1;
    }
   
    LPBYTE arrayData = (LPBYTE)MapViewOfFile(hDataMapping, FILE_MAP_READ |FILE_MAP_WRITE, 0, 0, ARRAY_LENGTH);
    if (arrayData == NULL) {
        printf("Failed to map view of file");
        CloseHandle(hDataMapping);
        CloseHandle(hDataMutex);
        return 1;
    }
   
    WaitForSingleObject(hDataMutex, INFINITE);
   
    BYTE data[ARRAY_LENGTH];
    __try {
        memcpy(data, arrayData, ARRAY_LENGTH);
        printf("\nNot sorted array: ");
        for (int i = 0; i < ARRAY_LENGTH; i++)
        {
            printf("%d ", data[i]);
        }
        for (int i = 1; i < ARRAY_LENGTH; i++) {
            BYTE key = data[i];
            int j = i - 1;
            while (j >= 0 && data[j] > key) {
                data[j + 1] = data[j];
                j--;
            }
            data[j + 1] = key;
        }
        memcpy(arrayData, data, ARRAY_LENGTH);
        printf("\nSorted array: ");
        for (int i = 0; i < ARRAY_LENGTH; i++)
        {
            printf("%d ", data[i]);
        }
    }
    __finally {
        ReleaseMutex(hDataMutex);
        UnmapViewOfFile(arrayData);
        CloseHandle(hDataMapping);
        CloseHandle(hDataMutex);
    }
    getchar();
}
