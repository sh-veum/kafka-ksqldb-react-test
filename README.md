Just goofing around with kafka and ksqldb.

Work in progress.

### docker-compose.py

Allows you to run `docker-compose` commands with the option to exclude specific containers.

### Usage

To run all containers except `auctionBackend`:

```bash
python script_name.py --exclude auctionBackend
```

To run all containers except auctionBackend and auctionDB and include the --build flag:

```bash
python script_name.py --exclude auctionBackend auctionDB --build
```
