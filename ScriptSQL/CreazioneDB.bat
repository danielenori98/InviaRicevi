@echo off

echo Avvio del container Oracle...
docker run -d --name oracle-db -p 1521:1521 -e ORACLE_PASSWORD=Esercizi_123 gvenzl/oracle-xe

echo Caricamento del database Oracle...
:wait_oracle_loop
docker exec oracle-db healthcheck.sh >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    timeout /t 2 /nobreak >nul
    goto wait_oracle_loop
)

echo Creazione tabella Oracle...
echo CREATE TABLE REGISTRO_MESSAGGI (ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY, TESTO VARCHAR2(4000), DATA_RICEZIONE TIMESTAMP DEFAULT CURRENT_TIMESTAMP); | docker exec -i oracle-db sqlplus -s SYSTEM/Esercizi_123@FREEPDB1

IF %ERRORLEVEL% EQU 0 (
    echo Tabella REGISTRO_MESSAGGI creata con successo su Oracle.
) ELSE (
    echo Errore durante la creazione della tabella.
)

echo --------------------------------------------------

set "CONTAINER_NAME=db2-db"
set "DB_NAME=mydb"
set "DB_USER=db2admin"
set "DB_PASS=Esercizi_123"

echo Avvio del container IBM Db2...
docker run -d --name %CONTAINER_NAME% -p 50000:50000 --privileged=true -e LICENSE=accept -e DB2INSTANCE=%DB_USER% -e DB2INST1_PASSWORD=%DB_PASS% -e DBNAME=%DB_NAME% -e BLU=false icr.io/db2_community/db2

echo Caricamento del database IBM Db2 (Potrebbe richiedere alcuni minuti)...
:wait_db2_loop
curl -s http://localhost:50000 >nul 2>&1
if %ERRORLEVEL% EQU 7 (
    timeout /t 2 /nobreak >nul
    goto wait_db2_loop
)

timeout /t 180 /nobreak >nul

echo Creazione tabella IBM Db2...
docker exec -i %CONTAINER_NAME% bash -c "export HOME=/database/config/%DB_USER% && su - %DB_USER% -c 'db2 connect to %DB_NAME% && db2 \"CREATE TABLE REGISTRO_MESSAGGI (ID INT GENERATED ALWAYS AS IDENTITY (START WITH 1, INCREMENT BY 1) PRIMARY KEY, TESTO VARCHAR(4000), DATA_RICEZIONE TIMESTAMP DEFAULT CURRENT_TIMESTAMP)\"'"

IF %ERRORLEVEL% EQU 0 (
    echo Tabella REGISTRO_MESSAGGI creata con successo su IBM Db2.
) ELSE (
    echo Errore durante la creazione della tabella.
)
pause