import cProfile
import primesGenerator

cProfile.run("primesGenerator.test(256)", sort="tottime")
