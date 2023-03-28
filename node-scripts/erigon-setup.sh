export ERIGON_DATADIR=./data/erigon-data
sudo mkdir -p $ERIGON_DATADIR
export PRYSM_DATADIR=./data/prysm-data
sudo mkdir -p $PRYSM_DATADIR
openssl rand -hex 32 | tr -d "\n" > $ERIGON_DATADIR/jwt.hex

docker-compose -f erigon-prysm-docker-compose.yaml up -d