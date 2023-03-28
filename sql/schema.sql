DROP DATABASE IF EXISTS eth_exp ON CLUSTER '{cluster}' SYNC;

CREATE DATABASE IF NOT EXISTS eth_exp ON CLUSTER '{cluster}';

---------------------------------------------------------------

DROP TABLE IF EXISTS eth_exp.block ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`block` ON CLUSTER '{cluster}' (
	`id` UUID,
	`block_num` UInt64 CODEC(Delta, LZ4),
	`miner` FixedString(42),
	`total_difficulty` String,
	`gas_limit` UInt64 CODEC(T64, LZ4),
	`gas_used` UInt64 CODEC(T64, LZ4),
	`base_fee_per_gas` Decimal(36, 18),
	`size_bytes` UInt64 CODEC(T64, LZ4),
	`hash` FixedString(66),
	`parent_hash` FixedString(66),
	`state_root` FixedString(66),
	`nonce` FixedString(18),
	`uncles` Array(FixedString(66)),
	`static_reward` Decimal(36, 18),
	`uncle_inclusion_reward` Decimal(36, 18),
	`burnt_fee` Decimal(36, 18),
	`total_tx_fee` Decimal(36, 18),
	`block_reward` Decimal(36, 18),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`total_tx_count` UInt32 CODEC(Delta, LZ4),
	`total_internal_tx_count` UInt32 CODEC(Delta, LZ4),
	`total_contract_creation_count` UInt32 CODEC(Delta, LZ4),
	`timestamp` UInt64 CODEC(Delta, LZ4)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY intDiv(`block_num`, 1000000)
ORDER BY (`block_num`, `id`)
SETTINGS index_granularity = 8192;

DROP TABLE IF EXISTS `eth_exp`.`block_queue_entry` SYNC;

CREATE TABLE `eth_exp`.`block_queue_entry` (
    `json` String
  ) ENGINE = RabbitMQ SETTINGS rabbitmq_host_port = 'rabbitmq-gate.c2b.lan:5672',
                            rabbitmq_exchange_name = 'BlockProcessedEvent',
                            rabbitmq_format = 'RawBLOB',
							rabbitmq_vhost = 'eth_exp',
                            rabbitmq_num_consumers = 1;

DROP VIEW IF EXISTS `eth_exp`.`block_view` SYNC;

CREATE MATERIALIZED VIEW IF NOT EXISTS `eth_exp`.`block_view`
TO `eth_exp`.`block` AS
WITH json SELECT
    JSONExtractRaw(JSONExtractRaw(json, 'data'), 'model') AS `data`,
	CAST(JSONExtract(`data`, 'id', 'String') as UUID) as id,
	CAST(JSONExtract(`data`, 'blockNum', 'String') as UInt64) as block_num,
	JSONExtract(`data`, 'miner', 'String') as miner,
	JSONExtract(`data`, 'totalDifficulty', 'String') as total_difficulty,
	CAST(JSONExtract(`data`, 'gasLimit', 'String') as UInt64) as gas_limit,
	CAST(JSONExtract(`data`, 'gasUsed', 'String') as UInt64) as gas_used,
	CAST(JSONExtract(`data`, 'baseFeePerGas', 'String') as Decimal(36, 18)) as base_fee_per_gas,
	CAST(JSONExtract(`data`, 'sizeBytes', 'UInt64') as UInt64) as size_bytes,
	JSONExtract(`data`, 'hash', 'String') as hash,
	JSONExtract(`data`, 'parentHash', 'String') as parent_hash,
	JSONExtract(`data`, 'stateRoot', 'String') as state_root,
	JSONExtract(`data`, 'nonce', 'String') as nonce,
	JSONExtract(`data`, 'uncles', 'Array(String)') as uncles,
	CAST(JSONExtract(`data`, 'staticReward', 'String') as Decimal(36, 18)) as static_reward,
	CAST(JSONExtract(`data`, 'uncleInclusionReward', 'String') as Decimal(36, 18)) as uncle_inclusion_reward,
	CAST(JSONExtract(`data`, 'burntFee', 'String') as Decimal(36, 18)) as burnt_fee,
	CAST(JSONExtract(`data`, 'totalTxFee', 'String') as Decimal(36, 18)) as total_tx_fee,
	CAST(JSONExtract(`data`, 'blockReward', 'String') as Decimal(36, 18)) as block_reward,
	CAST(JSONExtract(`data`, 'blockTimestamp', 'String') as UInt64) as block_timestamp,
	CAST(JSONExtract(`data`, 'totalTxCount', 'String') as UInt64) as total_tx_count,
	CAST(JSONExtract(`data`, 'totalInternalTxCount', 'String') as UInt64) as total_internal_tx_count,
	CAST(JSONExtract(`data`, 'totalContractCreationCount', 'String') as UInt64) as total_contract_creation_count,
	CAST(JSONExtract(`data`, 'timestamp', 'String') as UInt64) as `timestamp`
FROM `eth_exp`.`block_queue_entry`;

---------------------------------------------------------------

DROP TABLE IF EXISTS eth_exp.block_last ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`block_last` ON CLUSTER '{cluster}' (
	`block_num` UInt64 CODEC(Delta, LZ4),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`timestamp` UInt64 CODEC(Delta, LZ4)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`block_num`)
TTL FROM_UNIXTIME(`timestamp`) + INTERVAL 1 HOUR
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`block_last_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`block_last_view`
TO `eth_exp`.`block_last` AS
SELECT * FROM `eth_exp`.`block` ORDER BY block_num DESC;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`block_hash_num` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`block_hash_num` ON CLUSTER '{cluster}' (
	`block_num` UInt64 CODEC(Delta, LZ4),
	`hash` FixedString(66)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`hash`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`block_hash_num_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`block_hash_num_view`
TO `eth_exp`.`block_hash_num` AS
SELECT `block_num`, `hash` FROM `eth_exp`.`block`;

---------------------------------------------------------------

DROP VIEW IF EXISTS `eth_exp`.`block_mov_avg_view` ON CLUSTER '{cluster}' SYNC;

CREATE VIEW `eth_exp`.`block_mov_avg_view` ON CLUSTER '{cluster}'  
AS
SELECT t.date, 
round(avg(t.block_count) OVER (ORDER BY t.date Range BETWEEN 7 PRECEDING AND CURRENT ROW), 2) as block_count_avg,
round(avg(t.tx_count) OVER (ORDER BY t.date Range BETWEEN 7 PRECEDING AND CURRENT ROW), 2) as tx_count_avg
FROM 
(
	SELECT COUNT() block_count, SUM(total_tx_count) tx_count, toDate(FROM_UNIXTIME(block_timestamp)) date
	from eth_exp.block b
	group by date
) t
order by t.date desc
limit 365

---------------------------------------------------------------
---------------------------------------------------------------

DROP TABLE IF EXISTS eth_exp.`tx` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx` ON CLUSTER '{cluster}' (
	`id` UUID,
	`hash` FixedString(66),
	`error` String,
	`block_num` UInt64 CODEC(Delta, LZ4),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`from_address` FixedString(42),
	`to_address` FixedString(42),
	`created_contract_address` FixedString(42),	
	`value`Decimal(36, 18),
	`gas_used` UInt64  CODEC(T64, LZ4),
	`gas_price` Decimal(36, 18),
	`gas_limit` UInt64 CODEC(T64, LZ4),
	`base_fee_per_gas` Decimal(36, 18),
	`max_priority_fee_per_gas`Decimal(36, 18),
	`max_fee_per_gas` Decimal(36, 18),
	`total_fee` Decimal(36, 18),
	`nonce` UInt64 CODEC(DoubleDelta, LZ4),
	`index` UInt32 CODEC(DoubleDelta, LZ4),
	`type` UInt8 CODEC(DoubleDelta, LZ4),
	`total_internal_tx_count` UInt32 CODEC(Delta, LZ4),	
	`total_trace_count` UInt32 CODEC(Delta, LZ4),
	`timestamp` UInt64 CODEC(Delta, LZ4)	
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY intDiv(block_num, 1000000)
ORDER BY (`hash`, `id`)
SETTINGS index_granularity = 8192;

DROP TABLE IF EXISTS `eth_exp`.`tx_queue_entry` SYNC;

CREATE TABLE `eth_exp`.`tx_queue_entry` (
    `json` String
  ) ENGINE = RabbitMQ SETTINGS rabbitmq_host_port = 'rabbitmq-gate.c2b.lan:5672',
                            rabbitmq_exchange_name = 'TransactionProcessedEvent',
                            rabbitmq_format = 'RawBLOB',
							rabbitmq_vhost = 'eth_exp',
                            rabbitmq_num_consumers = 1;

DROP VIEW IF EXISTS `eth_exp`.`tx_view` SYNC;
							
CREATE MATERIALIZED VIEW IF NOT EXISTS `eth_exp`.`tx_view`
TO `eth_exp`.`tx` AS
WITH json SELECT
	JSONExtractRaw(JSONExtractRaw(json, 'data'), 'model') AS `data`,
	CAST(JSONExtract(`data`, 'id', 'String') as UUID) as id,
	JSONExtract(`data`, 'hash', 'String') as hash,
	JSONExtract(`data`, 'error', 'String') as error,
	CAST(JSONExtract(`data`, 'blockNum', 'String') as UInt64) as block_num,
	CAST(JSONExtract(`data`, 'blockTimestamp', 'String') as UInt64) as block_timestamp,
	JSONExtract(`data`, 'fromAddress', 'String') as from_address,
	JSONExtract(`data`, 'toAddress', 'String') as to_address,
	JSONExtract(`data`, 'createdContractAddress', 'String') as created_contract_address,
	CAST(JSONExtract(`data`, 'value', 'String') as Decimal(36, 18)) as `value`,
	CAST(JSONExtract(`data`, 'gasUsed', 'String') as UInt64) as gas_used,
	CAST(JSONExtract(`data`, 'gasPrice', 'String') as Decimal(36, 18)) as gas_price,
	CAST(JSONExtract(`data`, 'gasLimit', 'String') as UInt64) as gas_limit,
	CAST(JSONExtract(`data`, 'baseFeePerGas', 'String') as Decimal(36, 18)) as base_fee_per_gas,
	CAST(JSONExtract(`data`, 'maxPriorityFeePerGas', 'String') as Decimal(36, 18)) as max_priority_fee_per_gas,
	CAST(JSONExtract(`data`, 'maxFeePerGas', 'String') as Decimal(36, 18)) as max_fee_per_gas,
	CAST(JSONExtract(`data`, 'totalFee', 'String') as Decimal(36, 18)) as total_fee,
	CAST(JSONExtract(`data`, 'nonce', 'String') as UInt64) as `nonce`,
	CAST(JSONExtract(`data`, 'index', 'String') as UInt32) as `index`,
	CAST(JSONExtract(`data`, 'type', 'String') as UInt8) as `type`,
	CAST(JSONExtract(`data`, 'totalInternalTxCount', 'String') as UInt32) as `total_internal_tx_count`,
	CAST(JSONExtract(`data`, 'totalTraceCount', 'String') as UInt32) as `total_trace_count`,	
	CAST(JSONExtract(`data`, 'timestamp', 'String') as UInt64) as `timestamp`
FROM `eth_exp`.`tx_queue_entry`;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`tx_block_num_hash` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_block_num_hash` ON CLUSTER '{cluster}' (
	`block_num` UInt64 CODEC(Delta, LZ4),
	`hash` FixedString(66)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`block_num`, `hash`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`tx_block_num_hash_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`tx_block_num_hash_view`
TO `eth_exp`.`tx_block_num_hash` AS
SELECT `block_num`, hash FROM `eth_exp`.`tx`;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`tx_last` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_last` ON CLUSTER '{cluster}' (
	`block_num` UInt64 CODEC(Delta, LZ4),
	`index` UInt32 CODEC(DoubleDelta, LZ4),
	`hash` FixedString(66),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`timestamp` UInt64 CODEC(Delta, LZ4)
) ENGINE = ReplicatedReplacingMergeTree	
PARTITION BY tuple()
ORDER BY (`block_num`, `index`, `hash`)
TTL FROM_UNIXTIME(`timestamp`) + INTERVAL 1 HOUR
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`tx_last_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`tx_last_view`
TO `eth_exp`.`tx_last` AS
SELECT * FROM `eth_exp`.`tx` ORDER BY `block_num` DESC, `index` DESC;

---------------------------------------------------------------
---------------------------------------------------------------

DROP TABLE IF EXISTS eth_exp.`tx_token_transfer` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_token_transfer` ON CLUSTER '{cluster}' (
	`id` UUID,
	`tx_hash` FixedString(66),
	`block_num` UInt64 CODEC(Delta, LZ4),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`from_address` FixedString(42),
	`to_address` FixedString(42),
	`contract_address` FixedString(42),
	`contract_type` UInt8 CODEC(DoubleDelta, LZ4),
	`value` String,
	`index` UInt32 CODEC(DoubleDelta, LZ4),
	`timestamp` UInt64 CODEC(Delta, LZ4)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY intDiv(`block_num`, 1000000)
PRIMARY KEY (`id`)
ORDER BY (`id`)
SETTINGS index_granularity = 8192;

DROP TABLE IF EXISTS `eth_exp`.`tx_token_transfer_queue_entry` SYNC;

CREATE TABLE `eth_exp`.`tx_token_transfer_queue_entry` (
    `json` String
  ) ENGINE = RabbitMQ SETTINGS rabbitmq_host_port = 'rabbitmq-gate.c2b.lan:5672',
                            rabbitmq_exchange_name = 'TransactionTokenTransferProcessedEvent',
                            rabbitmq_format = 'RawBLOB',
							rabbitmq_vhost = 'eth_exp',					
                            rabbitmq_num_consumers = 1;
							
DROP VIEW IF EXISTS `eth_exp`.`tx_token_transfer_view` SYNC;
							
CREATE MATERIALIZED VIEW IF NOT EXISTS `eth_exp`.`tx_token_transfer_view`
TO `eth_exp`.`tx_token_transfer` AS
WITH json SELECT
	JSONExtractRaw(JSONExtractRaw(json, 'data'), 'model') AS `data`,
	CAST(JSONExtract(`data`, 'id', 'String') as UUID) as id,
	JSONExtract(`data`, 'txHash', 'String') as tx_hash,	
	CAST(JSONExtract(`data`, 'blockNum', 'String') as UInt64) as block_num,
	CAST(JSONExtract(`data`, 'blockTimestamp', 'String') as UInt64) as block_timestamp,
	JSONExtract(`data`, 'fromAddress', 'String') as from_address,
	JSONExtract(`data`, 'toAddress', 'String') as to_address,
	JSONExtract(`data`, 'contractAddress', 'String') as contract_address,
	CAST(JSONExtract(`data`, 'contractType', 'String') as UInt8) as contract_type,	
	JSONExtract(`data`, 'value', 'String') as `value`,
	CAST(JSONExtract(`data`, 'index', 'String') as UInt32) as `index`,	
	CAST(JSONExtract(`data`, 'timestamp', 'String') as UInt64) as `timestamp`
FROM `eth_exp`.`tx_token_transfer_queue_entry`;


---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`tx_token_transfer_ca_id` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_token_transfer_ca_id` ON CLUSTER '{cluster}' (
	`id` UUID,
	`contract_address` FixedString(42),
	`block_num` UInt64 CODEC(Delta, LZ4),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`index` UInt32 CODEC(DoubleDelta, LZ4)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`contract_address`, `block_timestamp`, `index`, `id`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`tx_token_transfer_ca_id_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`tx_token_transfer_ca_id_view`
TO `eth_exp`.`tx_token_transfer_ca_id` AS
SELECT `id`, `contract_address`, `block_num`, `block_timestamp`, `index` FROM `eth_exp`.`tx_token_transfer` where `to_address` is not null;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`tx_token_transfer_a_from_id` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_token_transfer_a_from_id` ON CLUSTER '{cluster}' (
	`id` UUID,
	`from_address` FixedString(42),
	`block_num` UInt64 CODEC(Delta, LZ4),
	`index` UInt32 CODEC(DoubleDelta, LZ4),
	`contract_type` UInt8 CODEC(DoubleDelta, LZ4),
	`contract_address` FixedString(42)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`from_address`, `block_num`, `index`, `id`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`tx_token_transfer_a_from_id_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`tx_token_transfer_a_from_id_view`
TO `eth_exp`.`tx_token_transfer_a_from_id` AS
SELECT `id`, `from_address`, `block_num`, `index`, `contract_type`, `contract_address` FROM `eth_exp`.`tx_token_transfer`;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`tx_token_transfer_a_to_id` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_token_transfer_a_to_id` ON CLUSTER '{cluster}' (
	`id` UUID,
	`to_address` FixedString(42),
	`block_num` UInt64 CODEC(Delta, LZ4),
	`index` UInt32 CODEC(DoubleDelta, LZ4),
	`contract_type` UInt8 CODEC(DoubleDelta, LZ4),
	`contract_address` FixedString(42)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`to_address`, `block_num`, `index`, `id`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`tx_token_transfer_a_to_id_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`tx_token_transfer_a_to_id_view`
TO `eth_exp`.`tx_token_transfer_a_to_id` AS
SELECT `id`, `to_address`, `block_num`, `index`, `contract_type`, `contract_address` FROM `eth_exp`.`tx_token_transfer` where `to_address` is not null;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`tx_token_transfer_tx_hash_id` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_token_transfer_tx_hash_id` ON CLUSTER '{cluster}' (
	`id` UUID,
	`tx_hash` FixedString(66)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`tx_hash`, `id`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`tx_token_transfer_tx_hash_id_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`tx_token_transfer_tx_hash_id_view`
TO `eth_exp`.`tx_token_transfer_tx_hash_id` AS
SELECT `id`, `tx_hash` FROM `eth_exp`.`tx_token_transfer`;


---------------------------------------------------------------
---------------------------------------------------------------

DROP TABLE IF EXISTS eth_exp.`tx_internal` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`tx_internal` ON CLUSTER '{cluster}' (
	`id` UUID,
	`tx_hash` FixedString(66),
	`block_num` UInt64 CODEC(Delta, LZ4),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`from_address` FixedString(42),
	`to_address` FixedString(42),
	`value`Decimal(36, 18),
	`gas_limit` UInt64 CODEC(Delta, LZ4),
	`error` String,
	`index` UInt32 CODEC(DoubleDelta, LZ4),
	`timestamp` UInt64 CODEC(Delta, LZ4)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY intDiv(`block_num`, 1000000)
ORDER BY (`tx_hash`, `id`)
SETTINGS index_granularity = 8192;

DROP TABLE IF EXISTS `eth_exp`.`tx_internal_queue_entry` SYNC;

CREATE TABLE `eth_exp`.`tx_internal_queue_entry` (
    `json` String
  ) ENGINE = RabbitMQ SETTINGS rabbitmq_host_port = 'rabbitmq-gate.c2b.lan:5672',
                            rabbitmq_exchange_name = 'TransactionInternalProcessedEvent',
                            rabbitmq_format = 'RawBLOB',
							rabbitmq_vhost = 'eth_exp',					
                            rabbitmq_num_consumers = 1;
							
DROP VIEW IF EXISTS `eth_exp`.`tx_internal_view` SYNC;
							
CREATE MATERIALIZED VIEW IF NOT EXISTS `eth_exp`.`tx_internal_view`
TO `eth_exp`.`tx_internal` AS
WITH json SELECT
	JSONExtractRaw(JSONExtractRaw(json, 'data'), 'model') AS `data`,
	CAST(JSONExtract(`data`, 'id', 'String') as UUID) as id,
	JSONExtract(`data`, 'txHash', 'String') as tx_hash,	
	CAST(JSONExtract(`data`, 'blockNum', 'String') as UInt64) as block_num,
	CAST(JSONExtract(`data`, 'blockTimestamp', 'String') as UInt64) as block_timestamp,
	JSONExtract(`data`, 'fromAddress', 'String') as from_address,
	JSONExtract(`data`, 'toAddress', 'String') as to_address,
	CAST(JSONExtract(`data`, 'value', 'String') as Decimal(36, 18)) as `value`,
	CAST(JSONExtract(`data`, 'gasLimit', 'String') as UInt64) as gas_limit,	
	JSONExtract(`data`, 'error', 'String') as error,
	CAST(JSONExtract(`data`, 'index', 'String') as UInt32) as `index`,
	CAST(JSONExtract(`data`, 'timestamp', 'String') as UInt64) as `timestamp`	
FROM `eth_exp`.`tx_internal_queue_entry`;

---------------------------------------------------------------
---------------------------------------------------------------

DROP TABLE IF EXISTS eth_exp.block_balance ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`block_balance` ON CLUSTER '{cluster}' (
	`id` UUID,
	`block_num` UInt64 CODEC(Delta, LZ4),
	`block_timestamp` UInt64 CODEC(Delta, LZ4),
	`tx_hash` FixedString(66),	
	`address` FixedString(42),
	`value` String,
	`contract_address` FixedString(42),
	`timestamp` UInt64 CODEC(Delta, LZ4)	
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY intDiv(`block_num`, 1000000)
PRIMARY KEY (`id`)
ORDER BY (`id`, `block_num`, `address`, `contract_address`, `tx_hash`)
TTL FROM_UNIXTIME(`timestamp`) + INTERVAL 1 HOUR
SETTINGS index_granularity = 8192;

DROP TABLE IF EXISTS `eth_exp`.`block_balance_queue_entry` SYNC;

CREATE TABLE `eth_exp`.`block_balance_queue_entry` (
    `json` String
  ) ENGINE = RabbitMQ SETTINGS rabbitmq_host_port = 'rabbitmq-gate.c2b.lan:5672',
                            rabbitmq_exchange_name = 'BlockBalanceProcessedEvent',
                            rabbitmq_format = 'RawBLOB',
							rabbitmq_vhost = 'eth_exp',
                            rabbitmq_num_consumers = 1;
							
DROP VIEW IF EXISTS `eth_exp`.`block_balance_view` SYNC;

CREATE MATERIALIZED VIEW IF NOT EXISTS `eth_exp`.`block_balance_view`
TO `eth_exp`.`block_balance` AS
WITH json SELECT
	JSONExtractRaw(JSONExtractRaw(json, 'data'), 'model') AS `data`,
	CAST(JSONExtract(`data`, 'id', 'String') as UUID) as id,
	CAST(JSONExtract(`data`, 'timestamp', 'String') as UInt64) as `timestamp`,
	CAST(JSONExtract(`data`, 'blockNum', 'String') as UInt64) as block_num,
	JSONExtract(`data`, 'txHash', 'String') as tx_hash,	
	CAST(JSONExtract(`data`, 'blockTimestamp', 'String') as UInt64) as block_timestamp,
	JSONExtract(`data`, 'address', 'String') as address,
	JSONExtract(`data`, 'value', 'String') as `value`,
	JSONExtract(`data`, 'contractAddress', 'String') as contract_address
FROM `eth_exp`.`block_balance_queue_entry`;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`address_last_balance_agg` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE `eth_exp`.`address_last_balance_agg` ON CLUSTER '{cluster}' (
	`address` FixedString(42),
	`contract_address` FixedString(42),
	`value` AggregateFunction(argMax, String, UInt64),
	`tx_hash` AggregateFunction(argMax, FixedString(66), UInt64),
	`block_number` AggregateFunction(max, UInt64),
	`block_timestamp` AggregateFunction(argMax, UInt64, UInt64),
	`total_tx_count` AggregateFunction(count, UInt64)
) ENGINE = ReplicatedAggregatingMergeTree
PARTITION BY tuple()
ORDER BY (`address`, `contract_address`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`address_last_balance_agg_view` ON CLUSTER '{cluster}' SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`address_last_balance_agg_view` ON CLUSTER '{cluster}'
TO `eth_exp`.`address_last_balance_agg` AS
SELECT `address`, `contract_address`, argMaxState(`value`, `block_num`) as `value`, argMaxState(`tx_hash`, `block_num`) as `tx_hash`,
maxState(`block_num`) as `block_number`, argMaxState(`block_timestamp`, `block_num`) as `block_timestamp`, countState() as `total_tx_count`
FROM `eth_exp`.`block_balance`
GROUP BY `address`, `contract_address`;

DROP VIEW IF EXISTS `eth_exp`.`address_last_balance_view` ON CLUSTER '{cluster}' SYNC;

CREATE VIEW `eth_exp`.`address_last_balance_view` ON CLUSTER '{cluster}'  
AS
SELECT `address`, `contract_address`, argMaxMerge(`value`) as `value`, argMaxMerge(`tx_hash`) as `tx_hash`, maxMerge(`block_number`) as `block_num`, argMaxMerge(`block_timestamp`) as `block_timestamp`, countMerge(`total_tx_count`) as `total_tx_count`
FROM `eth_exp`.`address_last_balance_agg_view`
GROUP BY `address`, `contract_address`;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`address_first_balance_change_agg` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE `eth_exp`.`address_first_balance_change_agg` ON CLUSTER '{cluster}' (
	`address` FixedString(42),
	`tx_hash` AggregateFunction(argMin, FixedString(66), UInt64),
	`value` AggregateFunction(argMin, String, UInt64),
	`contract_address` AggregateFunction(argMin, FixedString(42), UInt64),
	`block_number` AggregateFunction(min, UInt64),
	`block_timestamp` AggregateFunction(argMin, UInt64, UInt64)
) ENGINE = ReplicatedAggregatingMergeTree
PARTITION BY tuple()
ORDER BY (`address`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`address_first_balance_change_agg_view` ON CLUSTER '{cluster}' SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`address_first_balance_change_agg_view` ON CLUSTER '{cluster}'
TO `eth_exp`.`address_first_balance_change_agg` AS
SELECT `address`, argMinState(`tx_hash`, `block_num`) as `tx_hash`,  
argMinState(`value`, `block_num`) as `value`, 
argMinState(contract_address, `block_num`) as `contract_address`, 
minState(`block_num`) as `block_number`, argMinState(`block_timestamp`, `block_num`) as `block_timestamp`
FROM `eth_exp`.`block_balance`
GROUP BY `address`;

DROP VIEW IF EXISTS `eth_exp`.`address_first_balance_change_view` ON CLUSTER '{cluster}' SYNC;

CREATE VIEW `eth_exp`.`address_first_balance_change_view` ON CLUSTER '{cluster}'
AS
SELECT `address`, argMinMerge(`tx_hash`) as `tx_hash`, argMinMerge(`value`) as `value`, argMinMerge(`contract_address`) as `contract_address`, minMerge(`block_number`) as `block_num`, argMinMerge(`block_timestamp`) as `block_timestamp`
FROM `eth_exp`.`address_first_balance_change_agg_view`
GROUP BY `address`;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`token_holder_agg` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE `eth_exp`.`token_holder_agg` ON CLUSTER '{cluster}' (
	`contract_address` FixedString(42),
	`address` FixedString(42),
	`tx_hash` AggregateFunction(argMax, FixedString(66), UInt64),
	`value` AggregateFunction(argMax, String, UInt64),
	`block_number` AggregateFunction(max, UInt64),
	`block_timestamp` AggregateFunction(argMax, UInt64, UInt64)
) ENGINE = ReplicatedAggregatingMergeTree
PARTITION BY tuple()
ORDER BY (`contract_address`, `address`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`token_holder_agg_view` ON CLUSTER '{cluster}' SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`token_holder_agg_view` ON CLUSTER '{cluster}'
TO `eth_exp`.`token_holder_agg` AS
SELECT `contract_address`, `address`, 
argMaxState(`tx_hash`, `block_num`) as `tx_hash`, argMaxState(`value`, `block_num`) as `value`, 
maxState(`block_num`) as `block_number`, argMaxState(`block_timestamp`, `block_num`) as `block_timestamp`
FROM `eth_exp`.`block_balance`
GROUP BY `contract_address`, `address`;

DROP VIEW IF EXISTS `eth_exp`.`token_holder_view` ON CLUSTER '{cluster}' SYNC;

CREATE VIEW `eth_exp`.`token_holder_view` ON CLUSTER '{cluster}'
AS SELECT `contract_address`, `address`,
argMaxMerge(`tx_hash`) as `tx_hash`, argMaxMerge(`value`) as `value`,
maxMerge(`block_number`) as `block_num`, argMaxMerge(`block_timestamp`) as `block_timestamp`
FROM `eth_exp`.`token_holder_agg_view`
GROUP BY `contract_address`, `address`;

---------------------------------------------------------------
---------------------------------------------------------------

DROP TABLE IF EXISTS eth_exp.`contract` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`contract` ON CLUSTER '{cluster}' (
	`id` UUID,
	`creation_block_num` UInt64 CODEC(Delta, LZ4),
	`creation_block_timestamp` UInt64 CODEC(Delta, LZ4),
	`creation_tx_hash` FixedString(66),	
	`address` FixedString(42),	
	`decimals` UInt8,
	`symbol` String,
	`name` String,
	`total_supply` UInt64,
	`type` UInt8 CODEC(DoubleDelta, LZ4),
	`logo_url` String,
	`site_url` String,
	`timestamp` UInt64 CODEC(Delta, LZ4)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY intDiv(`creation_block_num`, 1000000)
PRIMARY KEY (`id`)
ORDER BY (`id`, `address`)
SETTINGS index_granularity = 8192;

DROP TABLE IF EXISTS `eth_exp`.`contract_queue_entry` SYNC;

CREATE TABLE `eth_exp`.`contract_queue_entry` (
    `json` String
  ) ENGINE = RabbitMQ SETTINGS rabbitmq_host_port = 'rabbitmq-gate.c2b.lan:5672',
                            rabbitmq_exchange_name = 'ContractCreationProcessedEvent',
                            rabbitmq_format = 'RawBLOB',
							rabbitmq_vhost = 'eth_exp',						
                            rabbitmq_num_consumers = 1;
							
DROP VIEW IF EXISTS `eth_exp`.`contract_view` SYNC;						
							
CREATE MATERIALIZED VIEW IF NOT EXISTS `eth_exp`.`contract_view`
TO `eth_exp`.`contract` AS
WITH json SELECT
	JSONExtractRaw(JSONExtractRaw(json, 'data'), 'model') AS `data`,
	CAST(JSONExtract(`data`, 'id', 'String') as UUID) as id,
	CAST(JSONExtract(`data`, 'timestamp', 'String') as UInt64) as `timestamp`,
	CAST(JSONExtract(`data`, 'creationBlockNum', 'String') as UInt64) as creation_block_num,
	CAST(JSONExtract(`data`, 'creationBlockTimestamp', 'String') as UInt64) as creation_block_timestamp,	
	JSONExtract(`data`, 'creationTxHash', 'String') as creation_tx_hash,	
	JSONExtract(`data`, 'address', 'String') as address,
	CAST(JSONExtract(`data`, 'decimals', 'String') as UInt8) as `decimals`,
	JSONExtract(`data`, 'symbol', 'String') as symbol,
	JSONExtract(`data`, 'name', 'String') as `name`,
	CAST(JSONExtract(`data`, 'totalSupply', 'String') as UInt64) as `total_supply`,
	CAST(JSONExtract(`data`, 'type', 'String') as UInt8) as `type`,		
	JSONExtract(`data`, 'logoUrl', 'String') as `logo_url`,
	JSONExtract(`data`, 'siteUrl', 'String') as `site_url`
FROM `eth_exp`.`contract_queue_entry`;

---------------------------------------------------------------

DROP TABLE IF EXISTS `eth_exp`.`contract_address_id` ON CLUSTER '{cluster}' SYNC;

CREATE TABLE eth_exp.`contract_address_id` ON CLUSTER '{cluster}' (
	`id` UUID,
	`address` FixedString(42)
) ENGINE = ReplicatedReplacingMergeTree
PARTITION BY tuple()
ORDER BY (`address`, `id`)
SETTINGS index_granularity = 8192;

DROP VIEW IF EXISTS `eth_exp`.`contract_address_id_view` SYNC;

CREATE MATERIALIZED VIEW `eth_exp`.`contract_address_id_view`
TO `eth_exp`.`contract_address_id` AS
SELECT `id`, `address` FROM `eth_exp`.`contract`;