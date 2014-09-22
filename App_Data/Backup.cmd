@ECHO OFF
SET target=%date%
COPY Database.sdf "DB_Backup\%target%.sdf"
