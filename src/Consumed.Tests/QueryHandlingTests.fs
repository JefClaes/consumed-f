module QueryHandlingTests

open System
open Consumed.Contracts
open Consumed.QueryHandling
open NUnit.Framework
open FsUnit

[<Test>]
let ``Quering list returns correct list``() =
    let read stream =
        [
            { 
                Stream = "consumed/1"; 
                Body = Event.Consumed 
                    {
                        Timestamp = new DateTime(2014, 1, 1);
                        Id = "1";
                        Category = "book";
                        Description = "Dune";
                        Url = "http://dunenovels.com"
                    }
            };
            { 
                Stream = "consumed/2"; 
                Body = Event.Consumed
                    {
                        Timestamp = new DateTime(2014, 1, 1);
                        Id = "2";
                        Category = "movie";
                        Description = "Gravity";
                        Url = "http://gravitymovie.com"
                    }
            };
            {
                Stream = "consumed/2";
                Body = Event.Removed
                    {
                        Timestamp = new DateTime(2014, 1, 1);
                        Id = "2"
                    } 
            }
        ]

    let actual = handleQuery read Query.List
  
    actual.Categories |> Seq.length |> should equal 1
    (actual.Categories |> Seq.head).Name |> should equal "book"
    (actual.Categories |> Seq.head).Items |> Seq.length |> should equal 1
    (actual.Categories |> Seq.head).Items |> Seq.head |> should equal 
        { 
            Id = "1";
            Timestamp = new DateTime(2014, 1, 1);
            Category = "book";
            Description = "Dune";
            Url = "http://dunenovels.com";
        }