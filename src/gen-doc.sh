set -ex

cd $(dirname $0)

MD_DIR=./Relic.Core/_g/docs
TARGET_DIR=./Relic.GatewayHost/wwwroot/doc/

cp -r $MD_DIR $TARGET_DIR

cd $TARGET_DIR
mkdocs build