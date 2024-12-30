#include <iostream>
#include <Windows.h>

constexpr int ARRAY_LENGTH = 25;
constexpr int MAX_STARS = 100;

HWND hWnd = NULL;
LRESULT CALLBACK WindowProc(HWND hwindow, UINT uMsg, WPARAM wParam, LPARAM lParam);
void CALLBACK TimerRoutine();

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
	const wchar_t CLASS_NAME[] = L"WindowProgram";
	WNDCLASS wc = {};
	wc.lpfnWndProc = WindowProc;
	wc.hInstance = hInstance;
	wc.lpszClassName = CLASS_NAME;
	RegisterClass(&wc);
	hWnd = CreateWindowEx(
		0,
		CLASS_NAME,
		L"Program_3",
		WS_OVERLAPPEDWINDOW, // window style
		CW_USEDEFAULT, // initial x position
		CW_USEDEFAULT, // initial y position
		CW_USEDEFAULT, // initial x size
		CW_USEDEFAULT, // initial y size
		NULL, // parent window handle
		NULL, // window menu handle
		hInstance, // program instance handle
		NULL); // creation parameters
	if (hWnd == NULL) {
		return 0;
	}
	ShowWindow(hWnd, nCmdShow);
	SetTimer(hWnd, 1, 500, (TIMERPROC)TimerRoutine);
	MSG msg = {};
	while (GetMessage(&msg, NULL, 0, 0)) {
		TranslateMessage(&msg);
		DispatchMessage(&msg);
	}
	return 0;
}
LRESULT CALLBACK WindowProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam) {
	switch (uMsg) {
		case WM_DESTROY:
			PostQuitMessage(0);
			return 0;
		case WM_PAINT: {
			PAINTSTRUCT ps;
			HDC hdc = BeginPaint(hwnd, &ps);
			FillRect(hdc, &ps.rcPaint, (HBRUSH)(COLOR_WINDOW + 1));
			HANDLE hDataMapping = OpenFileMappingW(FILE_MAP_READ | FILE_MAP_WRITE, false,L"MyDataMapping");
			if (hDataMapping == NULL) {
				EndPaint(hwnd, &ps);
				return 0;
			}
			HANDLE hDataMutex = OpenMutexW(SYNCHRONIZE, false, L"MyDataMutex");
			if (hDataMutex == NULL) {
				CloseHandle(hDataMapping);
				EndPaint(hwnd, &ps);
				return 0;
			}
			LPBYTE arrayData = (LPBYTE)MapViewOfFile(hDataMapping, FILE_MAP_READ | FILE_MAP_WRITE, 0, 0, ARRAY_LENGTH);
			if (arrayData == NULL) {
				CloseHandle(hDataMapping);
				CloseHandle(hDataMutex);
				EndPaint(hwnd, &ps);
				return 0;
			}
			WaitForSingleObject(hDataMutex, INFINITE);
			BYTE data[ARRAY_LENGTH];
			__try {
				memcpy(data, arrayData, ARRAY_LENGTH);
			}
			__finally {
				ReleaseMutex(hDataMutex);
				UnmapViewOfFile(arrayData);
				CloseHandle(hDataMapping);
				CloseHandle(hDataMutex);
			}
			wchar_t buffer[MAX_STARS + 1];
			for (int i = 0; i < ARRAY_LENGTH; i++) {
				for (int j = 0; j < data[i] && j < MAX_STARS; j++) {
					buffer[j] = L'*';
				}
				buffer[data[i]] = L'\0';
				TextOut(hdc, 10, 10 + i * 20, buffer, wcslen(buffer));
			}
			EndPaint(hwnd, &ps);
			return 0;
		}
	}
	return DefWindowProc(hwnd, uMsg, wParam, lParam);
}
void CALLBACK TimerRoutine()
{
	InvalidateRect(hWnd, NULL, TRUE);
}
