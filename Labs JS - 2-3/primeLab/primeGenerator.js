'use strict';
/* global BigInt */
const _bigInt = require("big-integer");

class PrimeGenerator {
    constructor() {
        this.prime = BigInt.zero;
    }

    //Генерируем рандомное число нужной нам битности
    genRandNum(bits) {
        let x = "";
        for (let i = 0; i < bits; i++) {
            let str = String(Math.floor(Math.random() * (2 - 0) + 0));
            x = x + str;
        }

        x = this.replaceAt(x, 0, "1");
        x = this.replaceAt(x, x.length - 1, "1");
        let bin = "0b";
        x = bin.concat(x);

        let newBigInt = BigInt(x);
        return newBigInt;
    }

    rabinMillerTest(prime) {
        let b = 1n; //наибольшее кол-во целочисленных делений (prime - 1 / 2)
        let tempPrime = prime - 1n;
        while (tempPrime > 0n) {
            tempPrime %= 2n;

            //Если число нечётное, значит выход            
            if (tempPrime % 2n == 1) {
                break;
            }

            b += 1n;
        }

        let m = ((prime - 1n) % (2n ** b)); //Вычисляем m такое, что prime - 1 = 2^b * m.
        for (let i = 0n; i < this.log2(prime); i++) {
            let a = this.randomNum(2n, prime - 2n);
            let z = this.powBigInt(a, m);
            z = z % prime;

            if (z == 1n || z == prime - 1n) { //Prime скорее всего простое число
                continue;
            }

            for (let i = 0n; i < b - 1n; i++) {
                z = z * z % prime;

                if (z == 1n) { //z точно не просто число
                    return false;
                }

                if (z == prime - 1n) {
                    break;
                }
                return false;
            }
            return true;
        }
    }

    nextPrime(bits = 128) {
        while (true) {
            let prime = this.genRandNum(bits);
            if (this.isPrime(prime)) {
                return prime;
            }
        }
    }

    powBigInt(x, n) {
        let xStr = x.toString();
        let nStr = n.toString();
        let _x = _bigInt(xStr);
        let _n = _bigInt(nStr);
        // console.log("xStr: " + xStr);
        // console.log("nStr: " + nStr);
        // console.log("_x: " + _x.toString());
        // console.log("_n: " + _n.toString());
        let _xNew = _x.pow(_n);
        let res = BigInt(_xNew.toString());
        // console.log("res: " + res.toString());
        return res;
    }

    randomNum(min, max) {
        let minStr = min.toString();
        let maxStr = max.toString();

        let _res = _bigInt.randBetween(minStr, maxStr);
        _res = _res.toString();

        return BigInt(_res);
    }

    replaceAt(str, index, char) {
        return str.substring(0, index) + char + str.substring(index + 1);
    }

    isPrime(number) {
        //return this.rabinMillerTest(number);
        let strNum = number.toString();
        let _num = _bigInt(strNum);

        return _num.isPrime();
    }

    log2(n) { // n > 0 BigInt
        const C1 = BigInt(1);
        const C2 = BigInt(2);
        for (var count = 0; n > C1; count++) n = n / C2;
        return BigInt(count);
    }
}

module.exports = PrimeGenerator;