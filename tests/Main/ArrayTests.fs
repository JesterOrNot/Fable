module Fable.Tests.Arrays

open System
open Util.Testing

type ParamArrayTest =
    static member Add([<ParamArray>] xs: int[]) = Array.sum xs

let add (xs: int[]) = ParamArrayTest.Add(xs)

#if FABLE_COMPILER
open Fable.Core

[<Emit("$1.constructor.name == $0")>]
let jsConstructorIs (s: string) (ar: 'T[]) = true
#endif

type ExceptFoo = { Bar:string }

let tests =
  testList "Arrays" [
    testCase "Pattern matching with arrays works" <| fun () ->
        match [||] with [||] -> true | _ -> false
        |> equal true
        match [|1|] with [||] -> 0 | [|x|] -> 1 | _ -> 2
        |> equal 1
        match [|"a";"b"|] with [|"a";"b"|] -> 1 | _ -> 2
        |> equal 1

    testCase "ParamArrayAttribute works" <| fun () ->
        ParamArrayTest.Add(1, 2) |> equal 3

    testCase "Passing an array to ParamArrayAttribute works" <| fun () ->
        ParamArrayTest.Add([|3; 2|]) |> equal 5

    testCase "Passing an array to ParamArrayAttribute from another function works" <| fun () ->
        add [|5;-7|] |> equal -2

    #if FABLE_COMPILER
    testCase "Typed Arrays work" <| fun () ->
        let xs = [| 1; 2; 3; |]
        let ys = [| 1.; 2.; 3.; |]
        let zs = [| "This is a string" |]
        xs |> jsConstructorIs "Int32Array" |> equal true
        ys |> jsConstructorIs "Float64Array" |> equal true
        zs |> jsConstructorIs "Array" |> equal true

(*
    testCase "Mapping from Typed Arrays work" <| fun () ->
        [| 1; 2; 3; |]
        |> Array.map string
        |> jsConstructorIs "Int32Array"
        |> equal false

    testCase "Mapping to Typed Arrays work" <| fun () ->
        [| "1"; "2"; "3"; |]
        |> Array.map int
        |> jsConstructorIs "Int32Array"
        |> equal true

        [| 1; 2; 3; |]
        |> Array.map float
        |> jsConstructorIs "Float64Array"
        |> equal true
*)
    #endif

(*
    testCase "Mapping from values to functions works" <| fun () ->
        let a = [| "a"; "b"; "c" |]
        let b = [| 1; 2; 3 |]
        let concaters1 = a |> Array.map (fun x y -> y + x)
        let concaters2 = a |> Array.map (fun x -> (fun y -> y + x))
        let concaters3 = a |> Array.map (fun x -> let f = (fun y -> y + x) in f)
        let concaters4 = a |> Array.map f
        let concaters5 = b |> Array.mapi f
        concaters1.[0] "x" |> equal "xa"
        concaters2.[1] "x" |> equal "xb"
        concaters3.[2] "x" |> equal "xc"
        concaters4.[0] "x" "y" |> equal "axy"
        concaters5.[1] "x" |> equal "12x"
        let f2 = f
        a |> Array.mapi f2 |> Array.item 2 <| "x" |> equal "2cx"

    testCase "Mapping from typed arrays to non-numeric arrays doesn't coerce values" <| fun () -> // See #120, #171
        let xs = map string [|1;2|]
        (box xs.[0]) :? string |> equal true
        let xs2 = Array.map string [|1;2|]
        (box xs2.[1]) :? string |> equal true

    testCase "Byte arrays are not clamped by default" <| fun () ->
        let ar = Util2.Helper2.CreateArray()
        ar.[0] <- ar.[0] + 255uy
        equal 4uy ar.[0]

    #if FABLE_COMPILER
    testCase "Clamped byte arrays work" <| fun () ->
        let ar = DllRef.Lib.createClampedArray()
        ar.[0] <- ar.[0] + 255uy
        equal 255uy ar.[0]
    #endif
    *)

    testCase "Array slice with upper index work" <| fun () ->
        let xs = [| 1; 2; 3; 4; 5; 6 |]
        let ys = [| 8; 8; 8; 8; 8; 8; 8; 8; |]
        xs.[..2] |> Array.sum |> equal 6
        xs.[..2] <- ys
        xs |> Array.sum |> equal 39

    testCase "Array slice with lower index work" <| fun () ->
        let xs = [| 1; 2; 3; 4; 5; 6 |]
        let ys = [| 8; 8; 8; 8; 8; 8; 8; 8; |]
        xs.[4..] |> Array.sum |> equal 11
        xs.[4..] <- ys
        xs |> Array.sum |> equal 26

    testCase "Array slice with both indices work" <| fun () ->
        let xs = [| 1; 2; 3; 4; 5; 6 |]
        let ys = [| 8; 8; 8; 8; 8; 8; 8; 8; |]
        xs.[1..3] |> Array.sum |> equal 9
        xs.[1..3] <- ys
        xs |> Array.sum |> equal 36

    testCase "Array slice with non-numeric arrays work" <| fun () ->
        let xs = [|"A";"B";"C";"D"|]
        xs.[1..2] <- [|"X";"X";"X";"X"|]
        equal xs.[2] "X"
        equal xs.[3] "D"

    testCase "Array literals work" <| fun () ->
        let x = [| 1; 2; 3; 4; 5 |]
        equal 5 x.Length

    testCase "Array indexer getter works" <| fun () ->
        let x = [| 1.; 2.; 3.; 4.; 5. |]
        x.[2] |> equal 3.

    testCase "Array indexer setter works" <| fun () ->
        let x = [| 1.; 2.; 3.; 4.; 5. |]
        x.[3] <- 10.
        equal 10. x.[3]

    testCase "Array getter works" <| fun () ->
        let x = [| 1.; 2.; 3.; 4.; 5. |]
        Array.get x 2 |> equal 3.

    testCase "Array setter works" <| fun () ->
        let x = [| 1.; 2.; 3.; 4.; 5. |]
        Array.set x 3 10.
        equal 10. x.[3]

    testCase "Array.Length works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs.Length |> equal 4

(*
    testCase "Array.zeroCreate works" <| fun () ->
        let xs = Array.zeroCreate 2
        equal 2 xs.Length
        equal 0 xs.[1]

    testCase "Array.create works" <| fun () ->
        let xs = Array.create 2 5
        equal 2 xs.Length
        Array.sum xs |> equal 10
    testCase "Array.blit works" <| fun () ->
        let xs = [|1..10|]
        let ys = Array.zeroCreate 20
        Array.blit xs 3 ys 5 4        // [|0; 0; 0; 0; 0; 4; 5; 6; 7; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0; 0|]
        ys.[5] + ys.[6] + ys.[7] + ys.[8] |> equal 22

    testCase "Array.copy works" <| fun () ->
        let xs = [|1; 2; 3; 4|]
        let ys = Array.copy xs
        xs.[0] <- 0                   // Ensure a deep copy
        ys |> Array.sum |> equal 10

    testCase "Array distinct works" <| fun () ->
        let xs =
            [| "b"
               "a"
               "b" |]
            |> Array.distinct
        xs.Length |> equal 2
        xs.[0] |> equal "b"
        xs.[1] |> equal "a"

    testCase "Array distinctBy works" <| fun () ->
        let xs =
            [| "a"
               "b"
               "a" |]
            |> Array.distinctBy(fun x -> x + "_")
        xs.Length |> equal 2
        xs.[0] |> equal "a"
        xs.[1] |> equal "b"

    testCase "Array distinctBy with tuples works" <| fun () ->
        let xs =
            [| (0,0),"a"
               (1,1),"b"
               (0,0),"c" |]
            |> Array.distinctBy(fun (x, _) -> x)
        xs.Length |> equal 2
        xs.[0] |> snd |> equal "a"
        xs.[1] |> snd |> equal "b"

    testCase "Array distinctBy works on large array" <| fun () ->
        let xs = [| 0 .. 50000 |]
        let ys =
            Array.append xs xs
            |> Array.distinctBy(fun x -> x.ToString())
        ys |> equal xs

    testCase "Array.sub works" <| fun () ->
        let xs = [|0..99|]
        let ys = Array.sub xs 5 10    // [|5; 6; 7; 8; 9; 10; 11; 12; 13; 14|]
        ys |> Array.sum |> equal 95

    testCase "Array.fill works" <| fun () ->
        let xs = Array.zeroCreate 4   // [|0; 0; 0; 0|]
        Array.fill xs 1 2 3           // [|0; 3; 3; 0|]
        xs |> Array.sum |> equal 6

    testCase "Array.empty works" <| fun () ->
        let xs = Array.empty<int>
        xs.Length |> equal 0

    testCase "Array.append works" <| fun () ->
        let xs1 = [|1; 2; 3; 4|]
        let zs1 = Array.append [|0|] xs1
        zs1.[0] + zs1.[1] |> equal 1
        let xs2 = [|"a"; "b"; "c"|]
        let zs2 = Array.append [|"x";"y"|] xs2
        zs2.[1] + zs2.[3] |> equal "yb"

    testCase "Array.average works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.average xs
        |> equal 2.5

    testCase "Array.averageBy works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.averageBy (fu x -> x * 2.) xs
        |> equal 5.

    testCase "Array.choose works" <| fun () ->
        let xs = [|1L; 2L; 3L; 4L|]
        let result = xs |> Array.choose (fun x ->
           if x > 2L then Some x
           else None)
        result.[0] + result.[1]
        |> equal 7L

    testCase "Array.collect works" <| fun () ->
        let xs = [|[|1|]; [|2|]; [|3|]; [|4|]|]
        let ys = xs |> Array.collect id
        ys.[0] + ys.[1]
        |> equal 3

        let xs1 = [|[|1.; 2.|]; [|3.|]; [|4.; 5.; 6.;|]; [|7.|]|]
        let ys1 = xs1 |> Array.collect id
        ys1.[0] + ys1.[1] + ys1.[2] + ys1.[3] + ys1.[4]
        |> equal 15.

    testCase "Array.concat works" <| fun () ->
        let xs = [|[|1.|]; [|2.|]; [|3.|]; [|4.|]|]
        let ys = xs |> Array.concat
        ys.[0] + ys.[1]
        |> equal 3.

    testCase "Array.exists works" <| fun () ->
        let xs = [|1u; 2u; 3u; 4u|]
        xs |> Array.exists (fun x -> x = 2u)
        |> equal true

    testCase "Array.exists2 works" <| fun () ->
        let xs = [|1UL; 2UL; 3UL; 4UL|]
        let ys = [|1UL; 2UL; 3UL; 4UL|]
        Array.exists2 (fun x y -> x * y = 16UL) xs ys
        |> equal true

    testCase "Array.filter works" <| fun () ->
        let xs = [|1s; 2s; 3s; 4s|]
        let ys = xs |> Array.filter (fun x -> x > 2s)
        ys.Length |> equal 2

    testCase "Array.find works" <| fun () ->
        let xs = [|1us; 2us; 3us; 4us|]
        xs |> Array.find ((=) 2us)
        |> equal 2us

    testCase "Array.findIndex works" <| fun () ->
        let xs = [|1.f; 2.f; 3.f; 4.f|]
        xs |> Array.findIndex ((=) 2.f)
        |> equal 1

    testCase "Array.findBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs |> Array.find ((>) 4.) |> equal 1.
        xs |> Array.findBack ((>) 4.) |> equal 3.

    testCase "Array.findIndexBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs |> Array.findIndex ((>) 4.) |> equal 0
        xs |> Array.findIndexBack ((>) 4.) |> equal 2

    testCase "Array.tryFindBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs |> Array.tryFind ((>) 4.) |> equal (Some 1.)
        xs |> Array.tryFindBack ((>) 4.) |> equal (Some 3.)
        xs |> Array.tryFindBack ((=) 5.) |> equal None

    testCase "Array.tryFindIndexBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs |> Array.tryFindIndex ((>) 4.) |> equal (Some 0)
        xs |> Array.tryFindIndexBack ((>) 4.) |> equal (Some 2)
        xs |> Array.tryFindIndexBack ((=) 5.) |> equal None

    testCase "Array.fold works" <| fun () ->
        let xs = [|1y; 2y; 3y; 4y|]
        let total = xs |> Array.fold (+) 0y
        total |> equal 10y

    testCase "Array.fold2 works" <| fun () ->
        let xs = [|1uy; 2uy; 3uy; 4uy|]
        let ys = [|1uy; 2uy; 3uy; 4uy|]
        let total = Array.fold2 (fun x y z -> x + y + z) 0uy xs ys
        total |> equal 20uy

    testCase "Array.foldBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let total = Array.foldBack (fun x acc -> acc - x) xs 0.
        total |> equal -10.

    testCase "Array.foldBack2 works" <| fun () ->
        let xs = [|1; 2; 3; 4|]
        let ys = [|1; 2; 3; 4|]
        let total = Array.foldBack2 (fun x y acc -> x + y - acc) xs ys 0
        total |> equal -4

    testCase "Array.forall works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.forall ((>) 5.) xs
        |> equal true

    testCase "Array.forall2 works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let ys = [|1.; 2.; 3.; 5.|]
        Array.forall2 (=) xs ys
        |> equal false
        Array.forall2 (fun x y -> x <= 4. && y <= 5.) xs ys
        |> equal true

    testCase "Array.init works" <| fun () ->
        let xs = Array.init 4 (float >> sqrt)
        xs.[0] + xs.[1]
        |> equal 1.
        (xs.[2] > 1. && xs.[3] < 2.)
        |> equal true
*)
    testCase "Array.isEmpty works" <| fun () ->
        Array.isEmpty [|"a"|] |> equal false
        Array.isEmpty [||] |> equal true

    testCase "Array.iter works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let total = ref 0.
        xs |> Array.iter (fun x ->
           total := !total + x
        )
        !total |> equal 10.

    testCase "Array.iter2 works" <| fun () ->
        let xs = [|1; 2; 3; 4|]
        let mutable total = 0
        Array.iter2 (fun x y ->
           total <- total - x - y
        ) xs xs
        total |> equal -20

    testCase "Array.iteri works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let total = ref 0.
        xs |> Array.iteri (fun i x ->
           total := !total + (float i) * x
        )
        !total |> equal 20.

    testCase "Array.iteri2 works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let total = ref 0.
        Array.iteri2 (fun i x y ->
           total := !total + (float i) * x + (float i) * y
        ) xs xs
        !total |> equal 40.

    testCase "Array.length works" <| fun () ->
        let xs = [|"a"; "a"; "a"; "a"|]
        Array.length xs |> equal 4

    testCase "Array.map works" <| fun () ->
        let xs = [|1.|]
        let ys = xs |> Array.map (fun x -> x * 2.)
        ys.[0] |> equal 2.

    testCase "Array.map doesn't execute side effects twice" <| fun () -> // See #1140
        let mutable c = 0
        let i () = c <- c + 1; c
        [| i (); i (); i () |] |> Array.map (fun x -> x + 1) |> ignore
        equal 3 c

    testCase "Array.map2 works" <| fun () ->
        let xs = [|1.|]
        let ys = [|2.|]
        let zs = Array.map2 (*) xs ys
        zs.[0] |> equal 2.

    testCase "Array.mapi works" <| fun () ->
        let xs = [|1.; 2.|]
        let ys = xs |> Array.mapi (fun i x -> float i + x)
        ys.[1] |> equal 3.

    testCase "Array.mapi2 works" <| fun () ->
        let xs = [|1.; 2.|]
        let ys = [|2.; 3.|]
        let zs = Array.mapi2 (fun i x y -> float i + x * y) xs ys
        zs.[1] |> equal 7.

    testCase "Array.mapFold works" <| fun () ->
        let xs = [|1y; 2y; 3y; 4y|]
        let result = xs |> Array.mapFold (fun acc x -> (x * 2y, acc + x)) 0y
        fst result |> Array.sum |> equal 20y
        snd result |> equal 10y

    testCase "Array.mapFoldBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let result = Array.mapFoldBack (fun x acc -> (x * -2., acc - x)) xs 0.
        fst result |> Array.sum |> equal -20.
        snd result |> equal -10.
    testCase "Array.max works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.max
        |> equal 2.

    testCase "Array.maxBy works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.maxBy (fun x -> -x)
        |> equal 1.

    testCase "Array.min works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.min
        |> equal 1.

    testCase "Array.minBy works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.minBy (fun x -> -x)
        |> equal 2.

    testCase "Array.ofList works" <| fun () ->
        let xs = [1.; 2.]
        let ys = Array.ofList xs
        ys.Length |> equal 2

    testCase "Array.ofSeq works" <| fun () ->
        let xs = seq { yield 1; yield 2 }
        let ys = Array.ofSeq xs
        ys.[0] |> equal 1

    testCase "Array.partition works" <| fun () ->
        let xs = [|1.; 2.|]
        let ys, zs = xs |> Array.partition (fun x -> x <= 1.)
        ys.[0] - zs.[0]
        |> equal -1.

    testCase "Array.permute works" <| fun () ->
        let xs = [|1.; 2.|]
        let ys = xs |> Array.permute (fun i -> i + 1 - 2 * (i % 2))
        ys.[0] |> equal 2.

    testCase "Array.pick works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.pick (fun x ->
           match x with
           | 2. -> Some x
           | _ -> None)
        |> equal 2.

    testCase "Array.range works" <| fun () ->
        [|1..5|]
        |> Array.reduce (+)
        |> equal 15
        [|0..2..9|]
        |> Array.reduce (+)
        |> equal 20

    testCase "Array.reduce works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs |> Array.reduce (-)
        |> equal -8.

    testCase "Array.reduceBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs |> Array.reduceBack (-)
        |> equal -2.

    testCase "Array.rev works" <| fun () ->
        let xs = [|1.; 2.|]
        let ys = xs |> Array.rev
        xs.[0] |> equal 1. // Make sure there is no side effects
        ys.[0] |> equal 2.

    testCase "Array.scan works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let ys = xs |> Array.scan (+) 0.
        ys.[2] + ys.[3]
        |> equal 9.

    testCase "Array.scanBack works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        let ys = Array.scanBack (-) xs 0.
        ys.[2] + ys.[3]
        |> equal 3.

    testCase "Array.sort works" <| fun () ->
        let xs = [|3; 4; 1; -3; 2; 10|]
        let ys = [|"a"; "c"; "B"; "d"|]
        xs |> Array.sort |> Array.take 3 |> Array.sum |> equal 0
        ys |> Array.sort |> Array.item 1 |> equal "a"

    testCase "Array.truncate works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.; 5.|]
        xs |> Array.truncate 2
        |> Array.last
        |> equal 2.

        xs.Length |> equal 5 // Make sure there is no side effects

        // Array.truncate shouldn't throw an exception if there're not enough elements
        try xs |> Array.truncate 20 |> Array.length with _ -> -1
        |> equal 5

    testCase "Array.sortDescending works" <| fun () ->
        let xs = [|3; 4; 1; -3; 2; 10|]
        xs |> Array.sortDescending |> Array.take 3 |> Array.sum |> equal 17
        xs.[0] |> equal 3  // Make sure there is no side effects

        let ys = [|"a"; "c"; "B"; "d"|]
        ys |> Array.sortDescending |> Array.item 1 |> equal "c"

    testCase "Array.sortBy works" <| fun () ->
        let xs = [|3.; 4.; 1.; 2.|]
        let ys = xs |> Array.sortBy (fun x -> -x)
        ys.[0] + ys.[1]
        |> equal 7.

    testCase "Array.sortWith works" <| fun () ->
        let xs = [|3.; 4.; 1.; 2.|]
        let ys = xs |> Array.sortWith (fun x y -> int(x - y))
        ys.[0] + ys.[1]
        |> equal 3.

    testCase "Array.sortInPlace works" <| fun () ->
        let xs = [|3.; 4.; 1.; 2.; 10.|]
        Array.sortInPlace xs
        xs.[0] + xs.[1]
        |> equal 3.

    testCase "Array.sortInPlaceBy works" <| fun () ->
        let xs = [|3.; 4.; 1.; 2.; 10.|]
        Array.sortInPlaceBy (fun x -> -x) xs
        xs.[0] + xs.[1]
        |> equal 14.

    testCase "Array.sortInPlaceWith works" <| fun () ->
        let xs = [|3.; 4.; 1.; 2.; 10.|]
        Array.sortInPlaceWith (fun x y -> int(x - y)) xs
        xs.[0] + xs.[1]
        |> equal 3.

    testCase "Array.sum works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.sum
        |> equal 3.

    testCase "Array.sumBy works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.sumBy (fun x -> x * 2.)
        |> equal 6.

    testCase "Array.toList works" <| fun () ->
        let xs = [|1.; 2.|]
        let ys = xs |> Array.toList
        ys.[0] + ys.[1]
        |> equal 3.

    testCase "Array.toSeq works" <| fun () ->
        let xs = [|1.; 2.|]
        let ys = xs |> Array.toSeq
        ys |> Seq.head
        |> equal 1.

    testCase "Array.tryFind works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.tryFind ((=) 1.)
        |> Option.isSome |> equal true
        xs |> Array.tryFind ((=) 3.)
        |> Option.isSome |> equal false

    testCase "Array.tryFindIndex works" <| fun () ->
        let xs = [|1.; 2.|]
        xs |> Array.tryFindIndex ((=) 2.)
        |> equal (Some 1)
        xs |> Array.tryFindIndex ((=) 3.)
        |> equal None

    testCase "Array.tryPick works" <| fun () ->
        let xs = [|1.; 2.|]
        let r = xs |> Array.tryPick (fun x ->
           match x with
           | 2. -> Some x
           | _ -> None)
        match r with
        | Some x -> x
        | None -> 0.
        |> equal 2.

    testCase "Array.unzip works" <| fun () ->
        let xs = [|1., 2.|]
        let ys, zs = xs |> Array.unzip
        ys.[0] + zs.[0]
        |> equal 3.

    testCase "Array.unzip3 works" <| fun () ->
        let xs = [|1., 2., 3.|]
        let ys, zs, ks = xs |> Array.unzip3
        ys.[0] + zs.[0] + ks.[0]
        |> equal 6.

    testCase "Array.zip works" <| fun () ->
        let xs = [|1.; 2.; 3.|]
        let ys = [|1.; 2.; 3.|]
        let zs = Array.zip xs ys
        let x, y = zs.[0]
        x + y |> equal 2.

    testCase "Array.zip3 works" <| fun () ->
        let xs = [|1.; 2.; 3.|]
        let ys = [|1.; 2.; 3.|]
        let zs = [|1.; 2.; 3.|]
        let ks = Array.zip3 xs ys zs
        let x, y, z = ks.[0]
        x + y + z |> equal 3.

(*
    testCase "Array as IList indexer has same behaviour" <| fun () ->
        let xs = [|1.; 2.; 3.|]
        let ys = xs :> _ IList
        ys.[0] <- -3.
        ys.[0] + ys.[2]
        |> equal 0.

    testCase "Array as IList count has same behaviour" <| fun () ->
        let xs = [|1.; 2.; 3.|]
        let ys = xs :> _ IList
        ys.Count |> equal 3

    testCase "Array as IList Seq.length has same behaviour" <| fun () ->
        let xs = [|1.; 2.; 3.|]
        let ys = xs :> _ IList
        ys |> Seq.length |> equal 3
*)
    testCase "Mapping with typed arrays doesn't coerce" <| fun () ->
        let data = [| 1 .. 12 |]
        let page size page data =
            data
            |> Array.skip ((page-1) * size)
            |> Array.take size
        let test1 =
            [| 1..4 |]
            |> Array.map (fun x -> page 3 x data)
        let test2 =
            [| 1..4 |]
            |> Seq.map (fun x -> page 3 x data)
            |> Array.ofSeq
        test1 |> Array.concat |> Array.sum |> equal 78
        test2 |> Array.concat |> Array.sum |> equal 78

    testCase "Array.item works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.item 2 xs |> equal 3.

    testCase "Array.tryItem works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.tryItem 3 xs |> equal (Some 4.)
        Array.tryItem 4 xs |> equal None
        Array.tryItem -1 xs |> equal None

    testCase "Array.head works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.head xs |> equal 1.

    testCase "Array.tryHead works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.tryHead xs |> equal (Some 1.)
        Array.tryHead [||] |> equal None

    testCase "Array.last works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        xs |> Array.last
        |> equal 4.

    testCase "Array.tryLast works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.tryLast xs |> equal (Some 4.)
        Array.tryLast [||] |> equal None

    testCase "Array.tail works" <| fun () ->
        let xs = [|1.; 2.; 3.; 4.|]
        Array.tail xs |> Array.length |> equal 3

    testCase "Array.groupBy returns valid array" <| fun () ->
        let xs = [|1; 2|]
        let actual = Array.groupBy (fun _ -> true) xs
        let actualKey, actualGroup = actual.[0]
        let worked = actualKey = true && actualGroup.[0] = 1 && actualGroup.[1] = 2
        worked |> equal true

    testCase "Array.except works" <| fun () ->
        Array.except [|2|] [|1; 3; 2|] |> Array.last |> equal 3
        Array.except [|2|] [|2; 4; 6|] |> Array.head |> equal 4
        Array.except [|1|] [|1; 1; 1; 1|] |> Array.isEmpty |> equal true
        Array.except [|'t'; 'e'; 's'; 't'|] [|'t'; 'e'; 's'; 't'|] |> Array.isEmpty |> equal true
        Array.except [|'t'; 'e'; 's'; 't'|] [|'t'; 't'|] |> Array.isEmpty |> equal true

        // TODO!!! Inject IEqualityComparer for non-primitive types
        // Array.except [|(1, 2)|] [|(1, 2)|] |> Array.isEmpty |> equal true
        // Array.except [|Map.empty |> (fun m -> m.Add(1, 2))|] [|Map.ofList [(1, 2)]|] |> Array.isEmpty |> equal true
        // Array.except [|{ Bar= "test" }|] [|{ Bar = "test" }|] |> Array.isEmpty |> equal true

    testCase "Array.[i] is undefined in Fable when i is out of range" <| fun () ->
        let xs = [|0|]
        #if FABLE_COMPILER
        isNull <| box xs.[1]
        #else
        try xs.[1] |> ignore; false with _ -> true
        #endif
        |> equal true

    testCase "Array iterators from range do rewind" <| fun () ->
        let xs = [|1..5|] |> Array.toSeq
        xs |> Seq.map string |> String.concat "," |> equal "1,2,3,4,5"
        xs |> Seq.map string |> String.concat "," |> equal "1,2,3,4,5"

    testCase "Array indexed works" <| fun () ->
        let xs = [|"a"; "b"; "c"|] |> Array.indexed
        xs.Length |> equal 3
        fst xs.[2] |> equal 2
        snd xs.[2] |> equal "c"

    testCase "Array.chunkBySize works" <| fun () ->
        Array.chunkBySize 4 [|1..8|]
        |> equal [| [|1..4|]; [|5..8|] |]
        Array.chunkBySize 4 [|1..10|]
        |> equal [| [|1..4|]; [|5..8|]; [|9..10|] |]

    testCase "Array.splitAt works" <| fun () ->
        let ar = [|1;2;3;4|]
        Array.splitAt 0 ar |> equal ([||], [|1;2;3;4|])
        Array.splitAt 3 ar |> equal ([|1;2;3|], [|4|])
        Array.splitAt 4 ar |> equal ([|1;2;3;4|], [||])

  ]