@echo off
cls
color f0
cd /d C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools
svcutil.exe http://localhost:9617/YardSaleService.svc?wsdl /d:E:\Documents\GitHub\Secret-Project\App\GarageSale\GarageSale\GarageSale /async /edb
rem cd /d C:\Program Files (x86)\Microsoft SDKs\Silverlight\v5.0\Tools

rem SlSvcUtil.exe http://localhost:9617/YardSaleService.svc?wsdl /d:E:\Documents\GitHub\Secret-Project\App\GarageSale\GarageSale\GarageSale

pause