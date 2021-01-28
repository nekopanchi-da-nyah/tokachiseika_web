<%@ Page language="C#" masterpagefile="../../page.master" %>
<asp:Content ContentPlaceHolderID="content" runat="server">
            <div id="customer" class="inner">
               <h2>お客様一覧</h2>
               <div class="search_area">
                  <form runat="server">
                     <ul>
                        <li>
                           <label>お客様CD</label>
                           <input type="text" name="code" />
                        </li>
                        <li>
                           <label>お客様名</label>
                           <input type="text" name="code" />
                        </li>
                     </ul>
                     <div>
                        <asp:button runat="server" text="検索"></asp:button>
                     </div>
                  </form>
               </div>
               <div>
                  <div class="output">
                     <ul>
                        <li></li>
                     </ul>
                  </div>
               </div>
            </div>
</asp:Content>