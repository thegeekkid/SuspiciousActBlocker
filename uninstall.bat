@echo off
title Removing ScamBlock
:main
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v install_location`) DO (
    set location=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\regedit.exe`) DO (
    set rege=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\System32\cmd.exe`) DO (
    set cm=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\System32\eventvwr.exe`) DO (
    set ev=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\System32\mmc.exe`) DO (
    set mc=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\System32\msconfig.exe`) DO (
    set confi=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\System32\notepad.exe`) DO (
    set np=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\System32\perfmon.exe`) DO (
    set prm=%%A %%B
    )
FOR /F "usebackq tokens=3*" %%A IN (`REG QUERY "HKEY_LOCAL_MACHINE\Software\Semrau Software Consulting\SuspiciousActBlocker" /v C:\Windows\System32\syskey.exe`) DO (
    set sk=%%A %%B
    )



if not %rege%==false (goto rmreg)
if not %cm%==false (goto rmcm)
if not %ev%==false (goto rmev)
if not %mc%==false (goto rmmc)
if not %confi%==false (goto rmconfi)
if not %np%==false (goto rmnp)
if not %prm%==false (goto rmprm)
if not %sk%==false (goto rmsk)

goto delRegs

:rmreg
echo Unprotecting regedit...
takeown /f C:\Windows\regedit.exe
icacls C:\Windows\regedit.exe /grant "%username%:F"
del /q C:\Windows\regedit.exe
rename %rege% regedit.exe
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\regedit.exe" /t REG_SZ /d "false"
goto main
:rmcm
echo Unprotecting cmd...
taskkill /f /im executor.exe
takeown /f C:\Windows\System32\cmd.exe
icacls C:\Windows\System32\cmd.exe /grant "%username%:F"
del /q C:\Windows\System32\cmd.exe.unprotected
del /q %cm%
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\System32\cmd.exe" /t REG_SZ /d "false"
goto main
:rmev
echo Unprotecting eventvwr...
takeown /f C:\Windows\System32\eventvwr.exe
icacls C:\Windows\System32\eventvwr.exe /grant "%username%:F"
del /q C:\Windows\System32\eventvwr.exe
rename %ev% eventvwr.exe
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\System32\eventvwr.exe" /t REG_SZ /d "false"
goto main
:rmmc
echo Unprotecting mmc...
takeown /f C:\Windows\System32\mmc.exe
icacls C:\Windows\System32\mmc.exe /grant "%username%:F"
del /q C:\Windows\System32\mmc.exe
rename %mc% mmc.exe
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\System32\mmc.exe" /t REG_SZ /d "false"
goto main
:rmconfi
echo Unprotecting msconfig...
takeown /f C:\Windows\System32\msconfig.exe
icacls C:\Windows\System32\msconfig.exe /grant "%username%:F"
del /q C:\Windows\System32\msconfig.exe
rename %confi% msconfig.exe
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\System32\msconfig.exe" /t REG_SZ /d "false"
goto main
:rmnp
echo Unprotecting notepad...
takeown /f C:\Windows\System32\notepad.exe
icacls C:\Windows\System32\notepad.exe /grant "%username%:F"
del /q  C:\Windows\System32\notepad.exe
rename %np% notepad.exe
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\System32\notepad.exe" /t REG_SZ /d "false"
goto main
:rmprm
echo Unprotecting perfmon...
takeown /f C:\Windows\System32\perfmon.exe
icacls C:\Windows\System32\perfmon.exe /grant "%username%:F"
del /q C:\Windows\System32\perfmon.exe
rename %rmprm% perfmon.exe
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\System32\perfmon.exe" /t REG_SZ /d "false"
goto main
:rmsk
echo Unprotecting syskey...
takeown /f C:\Windows\System32\syskey.exe
icacls C:\Windows\System32\syskey.exe /grant "%username%:F"
del /q C:\Windows\System32\syskey.exe
rename %sk% syskey.exe
reg add "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f /v "C:\Windows\System32\syskey.exe" /t REG_SZ /d "false"
goto main



:delRegs
echo Deleting registry entries...
reg delete "HKLM\Software\Semrau Software Consulting\SuspiciousActBlocker" /f
goto delFiles

:delFiles
echo Deleting files...
rmdir "%location%" /S /Q
echo.
echo.
echo.
echo ScamBlock has been uninstalled.
pause
