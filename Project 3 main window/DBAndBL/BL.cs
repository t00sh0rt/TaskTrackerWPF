﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Project_3
{
    public class BL
    {
        public BL() { }
        public static bool CheckLogin(string login)
        {
            User user = null;
            using (UserContext userContext = new UserContext()) 
            {   

                user= userContext.Users.Where(u=>u.login==login).FirstOrDefault();
            }
            if (user != null)
            {
                return true;
            }
            return false;
        }
        public static void Registration(string login,string password)
        {
            using (UserContext userContext = new UserContext())
            {
                User user = new User();
                user.login = login;
                user.password = password;
                userContext.Users.Add(user);
                userContext.SaveChanges();
            }
            
        }
        public static int LogIn(string login,string password)
        {
            User user = null;
            using (UserContext userContext=new UserContext())
            {

                user = userContext.Users.Where(u => u.login == login && u.password==password).FirstOrDefault();
            }
            if (user != null)
            {
                return user.id;
            }
            return 0;
        }
        public static void AddBoard(string boardName,int boardType,int userId)
        {
            int boardId;
            using (BoardContext boardContext = new BoardContext())
            {
                Board board = new Board();
                board.name= boardName;
                board.type= boardType;
                boardContext.Boards.Add(board);
                boardContext.SaveChanges();
                boardId = boardContext.Boards.ToArray()[boardContext.Boards.Count()-1].id;
            }
            AddBoardOwner(userId, boardId);

        }
        public static Board GetBoard(int boardId)
        {
            Board board=null;
            using (BoardContext boardContext = new BoardContext())
            {
                board = boardContext.Boards.Where(b => b.id == boardId).FirstOrDefault();
            }
            return board;
        }
        public static void AddBoardOwner(int userId,int boardId)
        {
            using(BoardOwnerContext boardOwnerContext=new BoardOwnerContext())
            {
                BoardOwner boardOwner=new BoardOwner();
                boardOwner.userId= userId;
                boardOwner.boardId= boardId;
                boardOwnerContext.BoardsOwners.Add(boardOwner);
                boardOwnerContext.SaveChanges();
            }
        }
        public static Board[] GetMyBoards(int userId)
        {
            List<Board> boards = new List<Board>();
            List<BoardOwner> boardOwners = new List<BoardOwner>();
            using (BoardOwnerContext boardOwnerContext = new BoardOwnerContext())
            {
                var bo = boardOwnerContext.BoardsOwners.Where(bo => bo.userId == userId);
                boardOwners = bo.ToList();


            }
            using (BoardContext boardContext = new BoardContext())
            {
                foreach (var obj in boardOwners)
                {
                    var b = boardContext.Boards.Where(b => b.id == obj.boardId).FirstOrDefault();
                    boards.Add(b);
                }
            }
            return boards.ToArray();
        }
        public static void RenameBoard(int boardId, string newName)
        {
            using (BoardContext boardContext = new BoardContext())
            {
                var llo = boardContext.Boards.Where(bo => bo.id == boardId).FirstOrDefault();
                llo.name=newName;
                
                boardContext.SaveChanges();
            }
        }
        public static void RemoveBoard(int boardId)
        {
            using (BoardOwnerContext boardOwnerContext = new BoardOwnerContext())
            {
                var llo= boardOwnerContext.BoardsOwners.Where(bo => bo.boardId == boardId);
                foreach (var he in llo)
                {
                    boardOwnerContext.BoardsOwners.Remove(he);
                }
                
                boardOwnerContext.SaveChanges();


            }
            using (BoardContext boardContext = new BoardContext())
            {
                var llo = boardContext.Boards.Where(b => b.id == boardId);
                foreach (var he in llo)
                {
                    boardContext.Boards.Remove(he);
                }

                boardContext.SaveChanges();
            }

        }
        public static void AddColumn(int boardId,string name)
        {
            Column column = new Column();
            column.name = name;
            column.boardId = boardId;
            using (ColumnContext  columnContext = new ColumnContext())
            {
                columnContext.Columns.Add(column);
                columnContext.SaveChanges();
            }
        }
        public static Column[] GetColumns(int boardId)
        {
            List<Column> columns = new List<Column>();
            using (ColumnContext columnContext = new ColumnContext())
            {
                    columns=(columnContext.Columns.Where(b => b.boardId == boardId)).ToList<Column>();
            }
            return columns.ToArray();
        }
        public static void RemoveColumn(int columnId)
        {
            
            using (ColumnContext columnContext = new ColumnContext())
            {
                var column = columnContext.Columns.Where(b => b.id == columnId).FirstOrDefault();
                columnContext.Columns.Remove(column);
                columnContext.SaveChanges();
            }
            using (TaskContext taskContext = new TaskContext())
            {
                var task=taskContext.MyTasks.Where(t=>t.columnId==columnId).FirstOrDefault();
                if (task != null){
                    taskContext.MyTasks.Remove(task); taskContext.SaveChanges();
                }
                
            }
        }
        public static void RenameColumn(int columnId, string newName)
        {
            using (ColumnContext columnContext = new ColumnContext())
            {
                var llo = columnContext.Columns.Where(bo => bo.id == columnId).FirstOrDefault();
                llo.name = newName;

                columnContext.SaveChanges();
            }
        }
        public static string GetUserLogin(int userId)
        {
            using(UserContext userContext = new UserContext())
            {
                var user=userContext.Users.Where(u=>u.id == userId).FirstOrDefault();
                return user.login;
            }
        }
        public static void AddTask(string name,int columnId)
        {
            MyTask task = new MyTask();
            task.name = name;
            task.columnId = columnId;
            using (TaskContext taskContext = new TaskContext())
            {
                taskContext.MyTasks.Add(task);
                taskContext.SaveChanges();
            }
        }
        public static MyTask[] GetTasks(int columnId)
        {
            List<MyTask> myTasks = new List<MyTask>();
            
                using (TaskContext taskContext = new TaskContext())
                {
                    var da = (taskContext.MyTasks.Where(b => b.columnId == columnId));
                    if (da.Count() > 0 && da != null)
                    {
                        myTasks = da.ToList<MyTask>();
                    }
                }
            
            
            return myTasks.ToArray();
        }
        public static void TaskDescEdit(string desc,int taskId)
        {
            using (TaskContext taskContext = new TaskContext())
            {
                    var da = taskContext.MyTasks.Where(d => d.id == taskId).FirstOrDefault();
                    da.description = desc;

                taskContext.SaveChanges();
            }
        }
        public static string GetDesc(int taskId)
        {
            using (TaskContext taskContext = new TaskContext())
            {
                var da = (taskContext.MyTasks.Where(b => b.id == taskId)).FirstOrDefault();
                return da.description;
            }
        }
        public static void DeleteTask(int taskId)
        {
            using (TaskContext taskContext = new TaskContext())
            {
                var da = (taskContext.MyTasks.Where(b => b.id == taskId)).FirstOrDefault();
                taskContext.Remove(da);
                taskContext.SaveChanges();
            }
        }
        public static void RenameTask(int taskId,string newName)
        {
            using (TaskContext taskContext = new TaskContext())
            {
                var da = (taskContext.MyTasks.Where(b => b.id == taskId)).FirstOrDefault();
                da.name=newName;
                taskContext.SaveChanges();
            }
        }

    }
    
}
