@echo off

docker run -d --name oracle-db -p 1521:1521 -e ORACLE_PASSWORD=Esercizi_123 gvenzl/oracle-xe

echo Attesa avvio effettivo del database...
:wait_loop
docker exec oracle-db healthcheck.sh >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    timeout /t 2 /nobreak >nul
    goto wait_loop
)

echo CREATE TABLE REGISTRO_MESSAGGI (ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY, TESTO VARCHAR2(4000), DATA_RICEZIONE TIMESTAMP DEFAULT CURRENT_TIMESTAMP); | docker exec -i oracle-db sqlplus -s SYSTEM/Esercizi_123@FREEPDB1

IF %ERRORLEVEL% EQU 0 (
    echo Tabella REGISTRO_MESSAGGI creata con successo.
) ELSE (
    echo Errore durante la creazione della tabella.
)

pause