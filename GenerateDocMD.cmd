cd src\CRMit.Customers\bin\Debug\net5.0
dotnet swagger tofile --output swagger.yml --serializeasv2 --yaml CRMit.Customers.dll v1
swagger-markdown -i swagger.yml -o swagger.md
cd ..\..\..\..\..
