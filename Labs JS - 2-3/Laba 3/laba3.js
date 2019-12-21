'use strict';
const _bigInt = require("big-integer");
const PrimeGenerator = require("../primeLab/primeGenerator");
const gen = new PrimeGenerator();

/* RSA (аббревиатура от фамилий Rivest, Shamir и Adleman) — 
криптографический алгоритм с открытым ключом, основывающийся
на вычислительной сложности задачи факторизации больших целых чисел. */

/* Реализация протокола RSA*/

let p = gen.nextPrime(8); //64 bit prime
let q = gen.nextPrime(8); //64 bit prime
let n = p * q;

let fi = (p - 1n) * (q - 1n); //значение функции Эйлера
let e = 17n; //открытая экспонента, 1 < e < fi - простые числа Ферма
let d = mulinv(e, fi); //закрытая экспонента, такая, что d*e = 1 (mod fi(n))
console.log("d = " + d);
let PK = [e, n]; //Пара (e, n) - открытый ключ
let SK = [d, n]; //Пара (d, n) - закрытый ключ

/* Шифровка, расшифровка */
let inData = "IVBO-01-16"; //вводные данные
let encryptText = encrypt(inData, PK); //шифруем данные
console.log("Зашифрованный текст: " + encryptText);

let decryptText = decrypt(encryptText, SK); //расшифровываем данные
console.log("Расшифрованный текст: " + decryptText);


//Расширенный алгоритм Евклида
//возвращает (gcd, x, y), такие, что a*x + b*y = g = gcd(a, b)
function xgcd(a, b) {
    if (a !== a || b !== b) {
        return [NaN, NaN, NaN];
    }
    if (a === Infinity || a === -Infinity || b === Infinity || b === -Infinity) {
        return [Infinity, Infinity, Infinity];
    }
    // Checks if a or b are decimals
    if ((a % 1n !== 0n) || (b % 1n !== 0n)) {
        return false;
    }
    var signX = (a < 0n) ? -1n : 1n,
        signY = (b < 0n) ? -1n : 1n,
        x = 0n,
        y = 1n,
        u = 1n,
        v = 0n,
        q, r, m, n;

    while (a !== 0n) {
        q = b / a;
        r = b % a;
        m = x - u * q;
        n = y - v * q;
        b = a;
        a = r;
        x = u;
        y = v;
        u = m;
        v = n;
    }
    let res = [b, signX * x, signY * y]
    return res;
}

//Нахождение модульной инверсии, для нахождения d(секретной экспоненты)
//возвращает x такое, что (x * a) % b == 1
function mulinv(a, b) {
    let g, x, _;
    let d = xgcd(a, b);
    console.log("d: " + d);
    g = d[0];
    x = d[1];
    _ = d[2];
    console.log("g: " + g);
    console.log("x: " + x);
    console.log("_: " + _);
    if (g == 1n) {
        return BigInt(x % b);
    }
}

//Посимвольная кодировка строки
function encrypt(data, PK) {
    let res = [];
    for (let i = 0; i < data.length; i++) {
        let el = gen.powBigInt(BigInt(ord(data[i])), PK[0]) % PK[1];
        console.log("el: " + el);
        res.push(el);
    }
    return res;
}

//Расшифрование массива данных
function decrypt(data, SK) {
    console.log("data: " + data);
    console.log("SK: " + SK);
    let res = "";
    for (let i = 0; i < data.length; i++) {
        let char = gen.powBigInt(BigInt(data[i]), BigInt(SK[0])) % SK[1];
        console.log("char: " + char);
        let el = String.fromCharCode(char.toString());
        res = res.concat(el);
    }
    return res;
}

//Преобразование символа в ascii код
function ord(string) {
    return string.charCodeAt(0);
}

// function xgcd(x, y, s1 = 1n, s2 = 0n, t1 = 0n, t2 = 1n) {
//     let q = x / y,
//         s1copy = s1,
//         t1copy = t1;
//     return (x % y === 0n) ? {
//         gcd: y,
//         s: s2,
//         t: t2
//     } : xgcd(y, x % y, s1 = s2, s2 = s1copy - q * s2, t1 = t2, t2 = t1copy - q * t2);
// }