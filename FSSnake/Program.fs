﻿// This is a .NET implementation of the console game snake.
// The game is written in F# and uses the .NET Console API for input and output.
// The game is a simple implementation of the classic snake game where the player controls a snake that grows in length as it eats food.
// The player loses if the snake runs into the walls or itself.
// The player can control the snake with the arrow keys.
module FSSnake      
open System
open System.Threading
open SFMLConsoleEmulator


let mutable running = true

let random = System.Random()

type GameState = {
    Direction: int * int
    Snake: (int * int) list
    Food: int * int
    Score: int
}


let mutable gameState = {
    Direction= (0,0)
    Snake= [(Console.WindowWidth/2, Console.WindowHeight/2)]
    Food= (0,0)
    Score= 0
}

let Controls = [
    ConsoleKey.LeftArrow, (-1, 0);
    ConsoleKey.RightArrow, (1, 0);
    ConsoleKey.UpArrow, (0, -1);
    ConsoleKey.DownArrow, (0, 1)
]

let drawBorder () =
    Console.SetCursorPosition(0, 0)
    Console.Write("+")
    Console.SetCursorPosition(Console.WindowWidth-1, 0)
    Console.Write("+")
    Console.SetCursorPosition(0, Console.WindowHeight-1)
    Console.Write("+")
    Console.SetCursorPosition(Console.WindowWidth-1, Console.WindowHeight-1)
    Console.Write("+")
    for i in 1..(Console.WindowWidth-2) do
        Console.SetCursorPosition(i, 0)
        Console.Write("-")
        Console.SetCursorPosition(i, Console.WindowHeight-1)
        Console.Write("-")
    for i in 1..(Console.WindowHeight-2) do
        Console.SetCursorPosition(0, i)
        Console.Write("|")
        Console.SetCursorPosition(Console.WindowWidth-1, i)
        Console.Write("|")

let rec generateFood gstate  =
    let loc = (random.Next(1, Console.WindowWidth-2), random.Next(1, Console.WindowHeight-2))
    if List.contains loc gstate.Snake then
        generateFood gstate 
    else
        {gstate with Food=loc}
 
let rec growSnake gstate =
    {gstate with Snake =
                    gstate.Snake @
                    [List.last gstate.Snake
                    |> fun (x, y) -> (x - fst gstate.Direction, y - snd gstate.Direction)]}
let tryEat gstate =
    if gstate.Snake.Head = gstate.Food then
        {gstate with Score=gstate.Score+1}
        |> generateFood
        |> growSnake
    else
        gstate

let moveSnake gstate =
    let newsnake =
        gstate.Snake
        |>List.map (fun (x, y) ->
            (x + (gstate.Direction |> fst) , y + (gstate.Direction |> snd )))
    {gstate with Snake = newsnake}


        

[<EntryPoint>]
let main argv =
    Console.SetWindowSize(80, 80)
    Console.CursorVisible <- false
    while running do
       Console.Clear()
       Console.SetCursorPosition(0, 0)
       drawBorder()
       Console.Display()
       Thread.Sleep(100)
    0     