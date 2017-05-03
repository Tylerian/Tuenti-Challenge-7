#!/usr/bin/env php
<?php

# Usage:
#  generate-password.php <user_id> <old_hash>

$secret1 = $argv[1];
$secret2 = $argv[2];

if (!isset($argv[4])) {
  # First password for this user
  $secret3 = crc32($argv[3]);
} else {
  # Existing user, password reset
  $secret3 = crc32($argv[4]);
}

echo 'Secret1: '.$secret1.PHP_EOL;
echo 'Secret2: '.$secret2.PHP_EOL;
echo 'Secret3: '.$secret3.PHP_EOL;

$counter = $secret3;

$counter = ($counter * bcpowmod($secret1, 10000000, $secret2)) % $secret2;

$password = "";
for ($i=0; $i<10; $i++) {
  echo 'pass counter: '. $counter .PHP_EOL;

  # Generate random passwords
  $counter = ($counter * $secret1) % $secret2;
  $password .= chr($counter % 94 + 33);
}

echo 'Hash: ' . md5($password).PHP_EOL;

$hash = md5($password);
echo "$password $hash\n";