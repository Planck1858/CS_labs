import socket
import random
import string
import sys
import os
import hashlib as h

sys.path.append(os.path.join(os.path.dirname(__file__), '../primelib'))
from primesGenerator import PGenerator

""" Secure Remote Password Protocol (SRPP) — 
протокол парольной аутентификации, устойчивый к прослушиванию и MITM-атаке и не требующий третьей доверенной стороны """

# hash функция
def get_hash(s):
    return int(h.sha256(str(s).encode()).hexdigest(), base=16)

# Заранее предопределено простое число и его модуль
N = 118803306835486195082399113213884973553715954127645643164791914472834727973460316682328992247504823866910116162950282167605383626397402784653263338255170149259032405011888091771332434517085211823401425148653358221163559402677267023285509402692432244623238207169953427535808510940208969293500011392939256398439
g = 2

sock = socket.socket()

# Клиент подключился к серверу
sock.connect((socket.gethostname(), 1234))
print("Добро пожаловать на сервер!")
way = -1
while way != 0:
    print("======================")
    print("1 - Зарегистрироваться")
    print("2 - Войти")
    print("0 - Закрыть программу")
    way = input(">> ")
    # Отправка запроса
    sock.sendall(way.encode())
    # Регистрация
    if way == '1':
        print("Регистрация нового пользователя: ")
        # Ввод логина и пароля для регистрации
        I = input("Логин: ")
        p = input("Пароль:")

        # Генерация соли
        s = ''.join((random.choice(string.ascii_letters)) for i in range(10))
        # Генерация x
        x = get_hash(s + p)
        # Генерация верификатора пароля
        v = pow(int(g), x, int(N))
        # Отправка серверу имени, соли, и верификатора
        sock.sendall(f"{I}\n{s}\n{v}".encode())

        ans = sock.recv(1024).decode("utf-8")
        if ans == "0":
            print(f"Ошибка! Пользователь {I} уже зарегистрирован")
        else:
            print("Регистрация завершена.")
    elif (way == "2"): # Вход
        print("Вход на сервер для зарегистрированных пользователей: ")
        # Ввод логина и пароля для входа
        I = input("Логин: ")
        p = input("Пароль:")

        # Генерация случайного числа а и A = g^a % N
        a = random.randint(1, 100)
        A = pow(g, a, N)

        # Отправка вычесленных выше данных серверу
        sock.sendall(f"{I}\n{A}\n{N}\n{g}".encode())

        # Получаем сразу ответ с солью и B
        B, s = sock.recv(1024).decode("utf-8").split('\n')
        if (B == '0' and s == '0'):
            print("Такого пользователя не существует!")
            continue

        # Вычислям скремблер(Сервер тоже)
        u = get_hash(str(A) + B)
        if u == 0:
            print("Ошибка: u == 0")
            break
        
        # Вычисления общего ключа сессии
        x = get_hash(s + p)
        # k = int(h.sha256((str(N) + str(g)).encode()).hexdigest(), base=16)
        k = get_hash(str(N) + str(g))
        S = pow(int(B) - k*pow(g, x, N), a + u*x, N)
        K = get_hash(str(S))
        #print(f"u = {u}\nk = {k}\nS = {S}\nK = {K}")

        # Генерация подтверждения для сервера 
        M = get_hash(
            str( get_hash(N) ^ get_hash(g) ) + \
            str(get_hash(I)) + s + str(A) + str(B) + str(K)
            )
        sock.sendall(str(M).encode())

        #print(M)

        # Повторная проверка кода со стороны сервера
        R = get_hash(str(A) + str(M) + str(K))
        R_from_server = sock.recv(1024).decode('utf-8')

        if (str(R) != R_from_server):
            print("Неверный пароль, попробуйте снова!")
            continue
        print("Пароль подтверждён!")
    
    else:
        sock.close()
        print("Выход...")
        break
    
