<%@ Page language="C#" masterpagefile="../../page.master" codefile="default.cs" inherits="Default" %>
<%@ MasterType VirtualPath="../../page.master" %>

<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="customer" class="inner">
               <h2>お客様一覧</h2>
               <div class="search_area">
                  <h3>検索条件</h3>
                  <form method="POST">
                     <ul>
                        <li>
                           <label>お客様CD</label>
                           <input type="text" name="code" value="<%= code %>" style="width: 6rem" />
                        </li>
                        <li>
                           <label>お客様名</label>
                           <input type="text" name="name" value="<%= name %>" style="width: 22rem" />
                        </li>
                     </ul>
                     <div class="btn">
                        <button name="search" value="true">検索</button>
                     </div>
                  </form>
               </div>
               <div>
                  <% if(dt.Rows.Count > 0){ %>
                  <div class="output">
                     <p>対象件数は <span><%= dt.Rows.Count %></span>件です</p>
                     <table>
                        <thead>
                           <tr>
                              <th colspan="2">お客様</th>
                              <th>締日</th>
                              <th colspan="2">回収サイト</th>
                              <th>回収日</th>
                           </tr>
                        </thead>
                        <tbody>
                           <% for(var i = 0; i < dt.Rows.Count; i++){ %>
                           <tr>
                              <td><%= dt.Rows[i]["お客様CD"] %></td>
                              <td><%= dt.Rows[i]["お客様名称"] %></td>
                              <td><%= dt.Rows[i]["締日"] %></td>
                              <td><%= dt.Rows[i]["回収SIGHTCD"] %></td>
                              <td><%= dt.Rows[i]["回収SIGHT"] %></td>
                              <td><%= dt.Rows[i]["回収日"] %></td>
                           </tr>
                           <% } %>
                        </tbody>
                     </table>
                  </div>
                  
                  <% }else if(dt.Rows.Count == 0){ %>
                  <% if(postback){ %>
                  <div class="output">
                     <p>出力できる内容がありません。検索条件をお確かめください。</p>
                  </div>
                  <% }} %>
               </div>
               <div class="btns">
                  <div>
                     <a href="entry.aspx"><button>新規</button></a>
                  </div>
                  <div>
                     <a href="../../dashboard.aspx"><button>終了</button></a>
                  </div>
               </div>
               <div class="breadcrumb">
                  <div>
                     <ul>
                        <li><a href="/">HOME</a> > </li>
                        <li>お客様一覧</li>
                     </ul>
                  </div>
               </div>
            </div>
</asp:Content>