@echo off

echo [ORACLE DATABASE]
echo SELECT * FROM REGISTRO_MESSAGGI; | docker exec -i oracle-db sqlplus SYSTEM/Esercizi_123@FREEPDB1

echo [IBM DB2]
docker exec -i db2-db bash -c "export HOME=/database/config/db2admin && su - db2admin -c 'db2 connect to mydb > /dev/null && db2 \"SELECT ID, substr(TESTO, 1, 50) AS TESTO, DATA_RICEZIONE FROM REGISTRO_MESSAGGI\" && db2 connect reset > /dev/null'"

pause
