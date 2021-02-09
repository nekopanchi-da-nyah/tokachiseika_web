<%@page language="C#" codefile="login.cs" inherits="Login.Program" %>
<!DOCTYPE>
<html lang="ja">
<head>
<meta charset="uft-8">
<meta name="viewport" content="width=device-width,initial-scale=1.0,minimum-scale=1,maximum-scale=1,user-scalable=no">
<title>とかち製菓 WEB発注システム|DEMOログイン</title>
<style>

body{
   margin: 0;
   padding: 0;
   background: #eee;
   display: flex;
   flex-flow: row nowrap;
   justify-content: center;
   align-items: center;
}

form{
   width: 280px;
   padding: 2rem 1rem;
   background: #fff;
   border-radius: 8px;
}

table{
   width: auto;
   margin: auto;
}

table caption{
   font-weight: bold;
   margin-bottom: 2rem;
   color: #58b976;
}

table td{
   width: 90%;
   margin: auto;
   display: flex;
   flex-flow: row nowrap;
   justify-content: center;
   align-items: stretch;
   height: 30px;
}

table label{
   width: 30%;
   background: #58b976;
   display: inline-flex;
   align-items: center;
   justify-content: center;
   color: #fff;
   font-weight: bold;
   border-radius: 5px 0 0 5px;
}

table input[type="text"], table input[type="password"]{
   width: 60%;
   height: 100%;
   border-radius: 0 5px 5px 0;
   border: 1px solid #58b976;
}

.login_btn{
   text-align: center;
}

</style>
</head>
<body>
   <form runat="server">
      <table>
         <caption>とかち製菓WEB発注システム</caption>
         <tbody>
            <tr>
               <td>
                  <label>ID</label>
                  <input type="text" name="login_id">
               </td>
            </tr>
            <tr>
               <td>
                  <label>PASS</label>
                  <input type="password" name="password">
               </td>
            </tr>
            <% if(msg != ""){ %>
            <tr>
               <td style="color: #f00; white-space: nowrap;"><%= msg %></td>
            </tr>
            <% } %>
         </tbody>
      </table>
      <div class="login_btn">
         <asp:button runat="server" text="ログイン" onclick="Click_Login"></asp:button>
      </div>
   </form>
</body>
</html>