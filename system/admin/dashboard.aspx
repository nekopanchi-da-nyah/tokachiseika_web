<%@ Page language="C#" masterpagefile="page.master" codefile="program/dashboard.cs" inherits="Dashboard.Program" %>
<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="dashboard">
               <h2>ダッシュボード</h2>
               <div class="output">
                  
                  <div class="box">
                     <h3 style="color: #f19524">新規（未処理）受注数</h3>
                     <p class="num">0<small>件</small></p>
                  </div>
                  
                  <div class="box">
                     <h3 style="color: #c74a4a">未出荷受注数</h3>
                     <p class="num">0<small>件</small></p>
                  </div>
                  
                  <div class="box">
                     <h3 style="color: #249cf1">先日受注高</h3>
                     <p class="num">0<small>円</small></p>
                  </div>
                  
                  <div class="box">
                     <h3 style="color: #a224f1">今月受注高</h3>
                     <p class="num">0<small>円</small></p>
                  </div>
                  
               </div>
               <h2>お知らせ</h2>
               <div class="info">
                  <p>デモ管理画面です。</p>
               </div>
            </div>
</asp:Content>