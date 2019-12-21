import socket
import hashlib as h
import random

''' def find_s_v(data):
    for line in data:
        log_s_v = line.split(';')
        if log_s_v[0] == I:
            s = log_s_v[1]
            v = log_s_v[2]
            return (s, v)
    return (None, None) '''

def get_hash(s):
    return int(h.sha256(str(s).encode()).hexdigest(), base=16)

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.bind((socket.gethostname(), 1234))
sock.listen(1)

print("Сервер запущен! В режиме ожидания клиентов")

way = -1
while way != 0:
    if way == -1:
        # Когда клиент подключается в 1 раз
        addr, port = sock.accept()
        print(f"[{port[0]}:{port[1]}] подключился к серверу")

    # Приём от отклиента сообщения о регистрации или о входе
    way = addr.recv(1024).decode("utf-8")

    # Регистрация
    if way == "1":   
        # Получение username, salt, password
        I, s, v = addr.recv(1024).decode("utf-8").split('\n')

        # Проверка, не зарегистрирован ли пользователь (в БД)
        with open("bd.txt", "r") as f:
            is_duplicate = False
            for line in f:
                if line.split(';')[0] == I:
                    print(f"Пользователь {I} уже зарегистрирован!")
                    addr.send("0".encode()) # Отправка неудачи при регистрации
                    is_duplicate = True
                    break
            # На новую итерацию, так как пользователь уже зарегистрирован
            if (is_duplicate):
                continue

        # Запись в БД полученных данных, проверенно новых
        with open("bd.txt", "a") as f:
            f.write(f"{I};{s};{v}\n")

        addr.send("1".encode()) # Отправка удачи при регистрации
        print(f"Пользователь {I} успешно зарегистрирован")

    elif way == "2": # Вход
        #Получение логина(I) и сгенерированного им А от пользователя + простого числааи модуля
        I, A, N, g = addr.recv(1024).decode("utf-8").split('\n')

        # Поиск соли и верификатора для введённого логина
        s, v = None, None
        with open("bd.txt", "r") as f:
            for line in f:
                log_s_v = line.split(';')
                if log_s_v[0] == I:
                    s = log_s_v[1]
                    v = int(log_s_v[2])
        
        if (s is None or v is None):
            print(F"Пользователь под логином {I} не зарегистрирован!")
            addr.send("0\n0".encode())
            sock.close()
            continue
        
        # Генерация B
        b = random.randint(1, 100)
        #k = int(h.sha256((N + g).encode()).hexdigest(), base=16)
        k = k = get_hash(str(N) + str(g))
        B = k*v + pow(int(g), b, int(N)) % int(N)
        #print(f"B = {B}\nk = {k}")

        # Отправка для пользователя B и соли
        addr.send(f"{B}\n{s}".encode())

        # Вычислям скремблер(Клиент тоже)
        u = get_hash(A + str(B))
        if u == 0:
            print("Ошибка: u == 0")
            addr.close()
            continue
        
        # Вычисления общего ключа сессии
        S = pow(int(A) * pow(v, u, int(N)), b, int(N))
        K = get_hash(str(S))

        #print(f"u = {u}\nk = {k}\nS = {S}\nK = {K}")
        
        # Получение подтверждения от клиента
        M_from_client = addr.recv(1024).decode('utf-8')

        # Генерация собственного подтверждения для сравнения с полученной
        M = get_hash(
            str( get_hash(N) ^ get_hash(g) ) + \
            str(get_hash(I)) + s + str(A) + str(B) + str(K)
            )

        #print(M)
        
        if (str(M) != M_from_client):
            print("Клиент ввёл неверный пароль")
            addr.send("0".encode())
            continue
        
        # Подтверждения для отправки клиенту
        R = get_hash(str(A) + str(M) + str(K))
        addr.send(str(R).encode())

        print(f"Уставлено соединение с пользователем {I} [{port[0]}:{port[1]}]")
    else:
        print(f"Клиент [{port[0]}:{port[1]}] отключился")
        addr.close()
        way = -1