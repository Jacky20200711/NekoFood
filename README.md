# 專案目標  
1.實作簡單的訂餐系統  
2.玩玩新出的 .NET 6 與 Visual Studio 2022  
&emsp;  
# 使用技術  
1.使用 AJAX + localStorage 實現自動登入  
2.使用 Entity Framework Core 存取資料庫  
3.使用 NLog 協助除錯  
4.使用 Cache + Session 來避免用戶重複登入，或是同時登入兩個以上的裝置  
5.使用 AJAX + JavaScript 的對話框，來簡化部分的操作流程  
&emsp;  
# 開發環境  
Win10(家用版) + Visual Studio 2022 + ASP.NET Core 6 MVC + SQL Server 2014 Express  
&emsp;  
# 安裝套件  
dotnet add package Microsoft.AspNetCore.Session --version 2.2.0  
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.0  
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.0  
dotnet add package NLog --version 4.7.12  
dotnet add package NLog.Web.AspNetCore --version 4.14.0-readme-preview  
&emsp;  
# 系統規劃  
1.網站管理員才能異動店家、商品、用戶  
2.群組建立者才能查看群組的[訂購情況]、修改訂單[已付款]的欄位  
3.一個群組只能對應一個店家  
4.一個訂單只能對應一個群組  
5.用戶可以建立群組、加入別人的群組、修改自己的密碼、異動自己的訂單  
&emsp;  
# DB Schema  
-- 會員(Name不能重複)  
CREATE TABLE UserAccount  
(  
&nbsp;&nbsp;&nbsp;&nbsp;[Id] [int] primary key NOT NULL IDENTITY,  
&nbsp;&nbsp;&nbsp;&nbsp;[Name] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[PasswordHash] [varchar](32) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[UserGroup] [nvarchar](10) NULL,  
);  
&emsp;  
-- 商店  
CREATE TABLE BentoShop  
(  
&nbsp;&nbsp;&nbsp;&nbsp;[Id] [int] primary key NOT NULL IDENTITY,  
&nbsp;&nbsp;&nbsp;&nbsp;[ShopGuid] [varchar](32) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Name] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Address] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Phone] [varchar](10) NOT NULL,  
);  
&emsp;  
-- 便當  
CREATE TABLE Bento  
(  
&nbsp;&nbsp;&nbsp;&nbsp;[Id] [int] primary key NOT NULL IDENTITY,  
&nbsp;&nbsp;&nbsp;&nbsp;[ShopGuid] [varchar](32) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Name] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Price] [int] NOT NULL,  
);  
&emsp;  
-- 便當群組  
CREATE TABLE BentoGroup  
(  
&nbsp;&nbsp;&nbsp;&nbsp;[Id] [int] primary key NOT NULL IDENTITY,  
&nbsp;&nbsp;&nbsp;&nbsp;[Creator] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[GroupGuid] [varchar](32) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[ShopGuid] [varchar](32) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Name] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[CreateTime] [datetime] NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[ExpireTime] [datetime] NOT NULL,  
);  
&emsp;  
-- 訂單  
CREATE TABLE BentoOrder  
(  
&nbsp;&nbsp;&nbsp;&nbsp;[Id] [int] primary key NOT NULL IDENTITY,  
&nbsp;&nbsp;&nbsp;&nbsp;[Payer] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[GroupGuid] [varchar](32) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[ShopGuid] [varchar](32) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[BentoName] [nvarchar](30) NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Number] [int] NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[TotalCost] [int] NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[IsChecked] [int] NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[CreateTime] [datetime] NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[ExpireTime] [datetime] NOT NULL,  
&nbsp;&nbsp;&nbsp;&nbsp;[Remark] [nvarchar](30) NULL,  
);  
&emsp;  
# 訂便當的流程01 - 用戶登入  
![image](https://github.com/Jacky20200711/NekoFood/blob/main/DEMO_01.PNG?raw=true)  
&emsp;  
# 訂便當的流程02 - 選擇要加入的群組  
![image](https://github.com/Jacky20200711/NekoFood/blob/main/DEMO_02.PNG?raw=true)  
&emsp;  
# 訂便當的流程03 - 填寫訂單  
![image](https://github.com/Jacky20200711/NekoFood/blob/main/DEMO_03.PNG?raw=true)  
&emsp;  
# 訂便當的流程04 - 查看訂單  
![image](https://github.com/Jacky20200711/NekoFood/blob/main/DEMO_04.PNG?raw=true)  
&emsp;  
# 群組建立者可以查看訂購情況  
![image](https://github.com/Jacky20200711/NekoFood/blob/main/DEMO_05.PNG?raw=true)  
&emsp;  
# 群組建立者可以查看或修改[已付款]欄位  
![image](https://github.com/Jacky20200711/NekoFood/blob/main/DEMO_06.PNG?raw=true)  
&emsp;  
