#!/bin/bash

# build handlers
dotnet restore
dotnet publish -c release

# create deployment package
pushd bin/release/netcoreapp1.0/publish
zip -r ./deploy-package.zip ./*
popd
