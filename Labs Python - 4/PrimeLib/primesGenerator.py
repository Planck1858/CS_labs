import random
import time
import math

#Генарация простых чисел с помощью теста Рабина-Мюллера
class PGenerator():
    def __init__(self):
        self.prime = ''
    
    # Генерация случайного числа
    # В параметрах выбирается длина в битах
    def __genRandNum(self, bits):
        x = ''
        # Генерация случайного бинарного большого числа 
        l = random.randint(bits, bits)
        for i in range(l):
            x += str(random.randint(0, 1))

        # Принудительное добавление '1' в первый и последний разряды,
        # чтобы число точно было нечётное и без незначащих нулей в начале
        x = '1' + x[1:-1] + '1'

        return int(x, base=2)

    # ТестРабина-Мюллера на простоту
    def __RabinMillerTest(self, prime):
        # Вычисление b - наибольшее кол-во целочисленных делений prime-1 на 2
        b = 1
        temp_prime = prime - 1
        while (temp_prime > 0):
            temp_prime //= 2
            # Если число нечётное, то на 2 без остатка разделить уже не сможем, значит выход
            if (temp_prime % 2 == 1): 
                break
            b += 1
        
        # Вычисление m такое, что prime - 1 = 2^b * m.
        # Заведомо известно, что все числа - целые, поэтому явно используем 
        # целочиселнное деление //, что позволит сохранить точность
        m = int( (prime - 1) // (2 ** b) )

        # Количество раундов(шагов) берётся порядка log2(prime)
        for step in range(len(str(prime))):
            # Выбор случайного а из отрезка [2, prime-2]
            a = random.randint(2, prime - 2)

            # Вычисляем a ^ m mod prime
            z = pow(a, m, prime)
            # Prime скорее простое число
            if (z == 1 or z == prime - 1):
                continue
            
            # Внутренний цикл выполняется b-1 раз
            for i in range(b - 1):
                z = z * z % prime

                # Точно не простое число    
                if (z == 1):
                    return False

                # Вернуться к внешнему циклу
                if (z == prime - 1):      
                    break
            return False
        return True

    # bits - длина числа в битах
    def nextPrime(self, bits=128):
        # Пока не нашли простое число генерируем новое и проверяем его тестом
        while True:
            prime = self.__genRandNum(bits)
            if (self.__RabinMillerTest(prime)):
                return prime
    
    def isPrime(self, number):
        return self.__RabinMillerTest(number)

def test(bits):
    pg = PGenerator()
    for i in range(10):
        s = time.time()
        prime = pg.nextPrime(bits)
        print(f'{i}:Prime = {prime}, time = {time.time() - s}')
