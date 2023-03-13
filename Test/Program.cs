// See https://aka.ms/new-console-template for more information
using MongoDB.Crud;
using System.Net.NetworkInformation;
using Test;

Console.WriteLine("Hello, World!");

using (Crud db = new Crud(GitIgnoredClass.Conn, GitIgnoredClass.DB))
{
    var rec = await db.LoadOneRecordByIndexAsync<UserModel>("Users", "UserName", "admin");
    Console.WriteLine(rec.UserName);
}
