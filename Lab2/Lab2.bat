@echo on

:: task2
if "%~1"=="" (
    echo Error: Provide log file name as the first argument.
    goto :eof
)
if "%~2"=="" (
   echo Error, provide a directory to clean and zip as the second argument
   goto :eof
)
if not exist "%~2" (
   echo Error, directory "%~2" does not exist
   goto :eof
)
if "%~3"=="" (
   echo Error: Provide a process name to terminate as the third argument
   goto :eof
)
if "%~4"=="" (
   echo Error, provide a destination directory for the archive as the fourth argument
   goto :eof
)
if "%~5"=="" (
   echo Error, provide an IP address to check as the fifth argument
   goto :eof
)
if "%~6"=="" (
   echo Error, provide a maximum log file size as the sixth argument
   goto :eof
)

::task3-4
set "logFile=%~1"
if not exist "%logFile%" (
    echo %date% %time% - Log file has been created > "%logFile%"
) else (
    echo %date% %time% - Log file has been opened >> "%logFile%"
)

::task5
w32tm /resync >> "%logFile%"
echo %date% %time% - Time has been synced with NTP server >> "%logFile%"

::task6 - список запущених процесів
echo %date% %time% - List of running processes: >> "%logFile%"
tasklist >> "%logFile%"

::task7
taskkill /f /im "%~3" >> "%logFile%"
echo %date% %time% - Terminated process "%~3". >> "%logFile%"

::task 8
set "tempDir=%~2"
echo %date% %time% - Removing temporary files in "%tempDir%". >> "%logFile%"
set /a fileCount=0
for /r "%tempDir%" %%f in (*.tmp temp*) do (
    del "%%f"
    set /a fileCount+=1
)
echo %date% %time% - %fileCount% temporary files removed. >> "%logFile%"

::task9-10
set "zipName=%date:~10,4%%date:~4,2%%date:~7,2%-%time:~0,2%%time:~3,2%%time:~6,2%.zip"
echo %date% %time% - Creating archive "%zipName%" from "%tempDir%". >> "%logFile%"
powershell -command "Compress-Archive -Path '%tempDir%\*' -DestinationPath '%zipName%'"
echo %date% %time% - Archive "%zipName%" created. >> "%logFile%"

::task11
set "destDir=%~4"
if not exist "%destDir%" (
    mkdir "%destDir%"
)
move "%zipName%" "%destDir%\" > nul
echo %date% %time% - Archive moved to "%destDir%". >> "%logFile%"

::task12-13
set "prevDate=%date:~10,4%%date:~4,2%%date:~7,2%"
if exist "%destDir%\%prevDate%*.zip" (
    echo %date% %time% - Yesterday's archive found. >> "%logFile%"
) else (
    echo %date% %time% - No archive from previous day. >> "%logFile%"
    echo %date% %time% - No archive from previous day. >> "email.txt"
)

::task14
forfiles /p "%destDir%" /s /d -30 /c "cmd /c del @path"
echo %date% %time% - Deleted archives older than 30 days from "%destDir%". >> "%logFile%"

::task15
ping -n 1 www.google.com > nul
if %errorlevel% equ 0 (
    echo %date% %time% - Internet is available. >> "%logFile%"
) else (
    echo %date% %time% - No internet connection detected. >> "%logFile%"
)

::task16
ping -n 1 "%~5" > nul
if %errorlevel% equ 0 (
    echo %date% %time% - Computer at IP "%~5" is online, shutting it down. >> "%logFile%"
    shutdown /m \\%~5 /s /f /t 0
) else (
    echo %date% %time% - No response from computer at IP "%~5". >> "%logFile%"
)

::task17 
echo %date% %time% - Networked computers: >> "%logFile%"
net view >> "%logFile%"

::task18
if exist ipon.txt (
    for /f "tokens=*" %%a in (ipon.txt) do (
        ping -n 1 %%a > nul
        if !errorlevel! neq 0 (
            echo %date% %time% - No reply from computer with IP %%a. >> "%logFile%"
            echo %date% %time% - No reply from computer with IP %%a. >> "email.txt"
        )
    )
) else (
    echo %date% %time% - ipon.txt not found. >> "%logFile%"
)

::task19
for %%f in ("%logFile%") do set size=%%~zf
if %size% gtr %~6 (
    echo %date% %time% - Log file size exceeds %~6 bytes. >> "%logFile%"
    echo %date% %time% - Log file size exceeds %~6 bytes. >> "email.txt"
)

::task20
echo %date% %time% - Disk space details: >> "%logFile%"
powershell -command "Get-Volume | Format-List -Property DriveLetter,SizeRemaining,Size" >> "%logFile%"

::task21
set "sysInfoFile=systeminfo_%date:~10,4%%date:~7,2%%date:~4,2%-%time:~0,2%%time:~3,2%%time:~6,2%.txt"
systeminfo > "%sysInfoFile%"


