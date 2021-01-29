<%@ Page language="C#" masterpagefile="../../page.master" codefile="program/edit.cs" inherits="Edit" %>
<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="customer" class="inner">
               <h2>お客様編集</h2>
               <% if(errMsg != ""){ %>
               <div class="errmsg">
                  <p style="color: red">エラーにより登録できませんでした。</p>
                  <p><%= errMsg %></p>
               </div>
               <% } %>
               <div class="entry_form">
                  <form method="POST">
                     <ul>
                        <li>
                           <label>お客様CD</label>
                           <input name="customer_code" type="text" style="width: 8rem;" required value="<%= wr["お客様CD"] %>" disabled>
                           <span style="color: red"><%= validMsg %></span>
                        </li>
                        <li>
                           <label>お客様名称</label>
                           <input name="customer_name" type="text" style="width: 22rem;" required value="<%= wr["お客様名称"] %>" >
                        </li>
                        <li>
                           <label>お客様略称</label>
                           <input name="short_name" type="text" style="width: 12rem;" value="<%= wr["お客様略称"] %>" >
                        </li>
                        <li>
                           <label>締日</label>
                           <input name="deadline" type="text" style="width: 4rem;" value="<%= wr["締日"] %>" >
                        </li>
                        <li>
                           <label>回収区分</label>
                           <select name="collection_code">
                              <option></option>
                              <% for(var i = 0; i < collectionCD.Rows.Count; i++){ %>
                              <% if((i + 1).ToString() == wr["回収区分CD"].ToString()){ %>
                              <option value="<%= (string)collectionCD.Rows[i]["CD"] %>" selected><span><%= (string)collectionCD.Rows[i]["CD"] + " : " + (string)collectionCD.Rows[i]["名称"] %><span></option>
                              <% }else{ %>
                              <option value="<%= (string)collectionCD.Rows[i]["CD"] %>"><span><%= (string)collectionCD.Rows[i]["CD"] + " : " + (string)collectionCD.Rows[i]["名称"] %><span></option>
                              <% }} %>
                           </select>
                        </li>
                        <li>
                           <label>回収サイト</label>
                           <select name="collection_sight">
                              <option></option>
                              <% for(var i = 0; i < collectionSight.Rows.Count; i++){ %>
                              <% if((i).ToString() == wr["回収SIGHTCD"].ToString()){ %>
                              <option value="<%= (string)collectionSight.Rows[i]["CD"] %>" selected><span><%= (string)collectionSight.Rows[i]["CD"] + " : " + (string)collectionSight.Rows[i]["名称"] %><span></option>
                              <% }else{ %>
                              <option value="<%= (string)collectionSight.Rows[i]["CD"] %>"><span><%= (string)collectionSight.Rows[i]["CD"] + " : " + (string)collectionSight.Rows[i]["名称"] %><span></option>
                              <% }} %>
                           </select>
                        </li>
                        <li>
                           <label>回収日</label>
                           <input name="collection_date" type="text" style="width: 4rem;" value="<%= wr["回収日"] %>">
                        </li>
                        <li>
                           <label>お客様メモ</label>
                           <textarea name="memo" rows="3" value="<%= wr["お客様MEMO"] %>"><%= wr["お客様MEMO"] %></textarea>
                        </li>
                     </ul>
                     <div class="btns">
                        <div>
                           <button class="trush" type="submit" name="submit" value="delete">削除</button>
                        </div>
                     
                        <div>
                           <button class="puls" type="submit" name="submit" value="update">更新</button>
                        </div>
                        
                        <div>
                           <button class="end"><a onclick="(function(){ window.open('', '_self').close(); })()">終了</a></button>
                        </div>
                     </div>
                  </form>
                  
               </div>
               
               <div class="breadcrumb">
                  <div>
                     <ul>
                        <li><a href="/">HOME</a> > </li>
                        <li><a href="default.aspx">お客様一覧</a> ></li>
                        <li>お客様新規登録</li>
                     </ul>
                  </div>
               </div>
            </div>
</asp:Content>