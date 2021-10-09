#!/bin/bash

set -e

terraform validate

set +e
terraform plan -detailed-exitcode

terraform_exitcode=$?
echo "Terraform plan exited with status code ${terraform_exitcode}"

if [ ${terraform_exitcode} -eq 0 ]; then
    echo "No changes detected"
else
    if [ ${terraform_exitcode} -eq 2 ]; then
    echo "Terraform succeeded with updates"
    exit 0
    else
    echo "ERROR: terraform exited with code ${terraform_exitcode}"
    exit 1
    fi
fi