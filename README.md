<div align="center" style="text-align: center; width: 100%">
<img src="assets/banana-blaster.png" height="100px"/>
<h1>BananaBlaster</h1>

The BananaBlaster is a small project that implements Bit-Blasting in C# with the usage of the Z3 SAT-Solver.
</div>

## Usage
This program is a CLI-Tool that can check formulas with Bit-Vector-Arithmetic:
```sh
./BananaBlaster "(x - 2) > 1 => false"
```

The program uses normal bit-blasting by default. To enable incremenal bit-blasting the flag `--incremental` or `--inc` has to be enabled:
```sh
./BananaBlaster "(x - 2) > 1 => false" --incremental
```

---

By default the program only outputs if the formula is `SATISFIABLE` or not.
To get more information, it is possible to use the `--verbose` or `-v` flag that adds different metadata like the values of the identifiers and the time the solver took.
```sh
./BananaBlaster "(x - 2) > 1 => false" --verbose
```

## License
This project is licensed under the [MIT-License](LICENSE).
