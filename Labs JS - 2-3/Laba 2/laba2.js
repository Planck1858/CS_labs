'use strict';
//const bigInt = require("big-integer");
const PrimeGenerator = require("../primeLab/primeGenerator");
const gen = new PrimeGenerator();

class User1 {
    constructor(name) {
        this.name = name;
        this.p_PublicKey = gen.nextPrime(32); //32 bit Prime
        this.g_PublicKey = gen.randomNum(3n, 15n);
        this.a_SecretKey = gen.genRandNum(8); //8 bit a secr key
        this.A_publicKey = 0n;
        this.Key = 0n;
    }
}

class User2 {
    constructor(name) {
        this.name = name;
        this.b_SecretKey = gen.genRandNum(8); //8 bit a secr key
        this.B_publicKey = 0n;
        this.Key = 0n;
    }
}

let Alice = new User1("Alice");
let Bob = new User2("Bob");
let p = Alice.p_PublicKey;
let g = Alice.g_PublicKey;

Alice.A_publicKey = gen.powBigInt(g, Alice.a_SecretKey) % p;
Bob.B_publicKey = gen.powBigInt(g, Bob.b_SecretKey) % p;

Alice.Key = gen.powBigInt(Bob.B_publicKey, Alice.a_SecretKey) % p;
Bob.Key = gen.powBigInt(Alice.A_publicKey, Bob.b_SecretKey) % p;

if (Alice.Key == Bob.Key) {
    console.log("Ключи равны");
} else {
    console.log("Ключи не равны");
}