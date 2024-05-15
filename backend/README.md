# Remember for later:

### To get a stream of bids in an auction:

```sql
SELECT b.*, a.*
FROM AUCTION_BIDS b
JOIN AUCTIONS a
ON b.AUCTION_ID = a.AUCTION_ID
EMIT CHANGES;
```

### Docs'n stuff:

[ksqlDB.RestApi.Client-DotNet](https://github.com/tomasfabian/ksqlDB.RestApi.Client-DotNet)
