#!/bin/bash
#
#  Go-lang-Vcs-NEXTClan 
#
set -e


TAGS=${TAGS:-all}

PWD=`pwd`
cd test/bdd

echo "---
Important: for these test to run correctly, you must ensure that your hosts file has the following entries:
127.0.0.1 testnet.orb.local
---
"
echo "Running vcs integration tests with tag=$TAGS"


go test -count=1 -v -cover . -p 1 -timeout=40m $TAGS

cd $PWD
