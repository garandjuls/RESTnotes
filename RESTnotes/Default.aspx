<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="RESTnotes.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <h1>Note App using REST API</h1>
        <div>&nbsp;</div>
                <div style="border:1px solid black;">
                    Using cURL... <br />
<br />post new note
<br />curl -i -H "Content-Type: application/json" -X POST -d "{\"body\" : \"Pick up milk!\"}" http://localhost/api/notes
<br />get existing note
<br />curl -i -H "Content-Type: application/json" -X GET http://localhost/api/notes/1
<br />get all notes
<br />curl -i -H "Content-Type: application/json" -X GET http://localhost/api/notes
<br />update a note
<br />curl -i -H "Content-Type: application/json" -X PUT -d "{\"id\":1,\"body\" : \"First Noted\"}" http://localhost/api/notes
<br />delete a note
<br />curl -i -H "Content-Type: application/json" -X DELETE http://localhost/api/notes/1
      <br />      
                    </div>
                    <div>&nbsp;</div>
        <div style="border:1px solid black;">
        This area contains Ajax CRUD functions<br /><br />
        <span style="color:red;">*</span>=Required field
        <div>
            Root URL
            <input type="text" id="txtpath" value="http://localhost:63929/api/notes" style="width:80%" />
        </div>
        <div style="border:1px solid black;">
            <h1>Create</h1>
            Body<span style="color:red;">*</span><input type="text" id="txtbody1" />
            <input type="button" value="Create (Post)" onclick="notecreate()" />
        </div>
        <div style="border:1px solid black;">
            <h1>Read</h1>
            ID<input type="text" id="txtid2" />
            Filter<input type="text" id="txtfilter" />
            <input type="button" value="Read (Get)" onclick="noteread()" />
        </div>  
        <div style="border:1px solid black;">
            <h1>Update</h1>
            ID<span style="color:red;">*</span><input type="text" id="txtid3" />
            Body<span style="color:red;">*</span><input type="text" id="txtbody3" />
            <input type="button" value="Update (Put)" onclick="noteupdate()" />
        </div>
        <div style="border:1px solid black;">
            <h1>Delete</h1>
            ID<span style="color:red;">*</span><input type="text" id="txtid4" />
            <input type="button" value="Delete (Delete)" onclick="notedelete()" />
        </div>

        Formatted response
        <table id="table1" border="1" title="All Notes">
            <thead >
                <tr>
                    <td>ID</td>
                    <td>Body</td>
                </tr>
            </thead>
        </table>

        <div>
            Raw response
        </div>
        <div>
            <input type="text" id="txtraw" style="width:80%;" />
        </div>
    </div>
        </div>


    <script type="text/javascript">
        // do async
        var xhttp;
        // url to post to
        function syncJsonPost(purl, pmethod, pprms) {
            if (window.XMLHttpRequest) {
                xhttp = new XMLHttpRequest();
            } else {
                // code for IE6, IE5
                xhttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            xhttp.onreadystatechange = function () {
                if (xhttp.readyState == 4 && xhttp.status == 200) {
                    showit(xhttp.responseText);
                }
            };
            xhttp.open(pmethod, purl, false);

            xhttp.setRequestHeader("Authorization", "None");
            xhttp.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            xhttp.send(pprms);
        }

        // populates the fields
        function showit(ptext) {
            document.getElementById("txtraw").value = ptext;
            var myObj = JSON.parse(ptext);
            var table = document.getElementById("table1");
            var isarray = false;            
            if (myObj.notes)
                isarray = true;

            if (! isarray) {
                var row = table.insertRow(table.rows.length);
                var cell1 = row.insertCell(0);
                var cell2 = row.insertCell(1);

                cell1.innerHTML = myObj.id;
                cell2.innerHTML = myObj.body;
            }
            else {
                for (var iob=0; iob < myObj.notes.length; iob++) {
                    var row = table.insertRow(table.rows.length);
                    var cell1 = row.insertCell(0);
                    var cell2 = row.insertCell(1);

                    var thisobj = myObj.notes[iob];
                    cell1.innerHTML = thisobj.id;
                    cell2.innerHTML = thisobj.body;
                }
            }
        }


        function noteread() {
            var root = document.getElementById("txtpath").value;
            var pid=document.getElementById("txtid2").value;
            var pbody=document.getElementById("txtfilter").value;
            var prs = {
                "id": pid,
                "body": pbody
            };
//            syncJsonPost(root, "GET", JSON.stringify(prs)); // JSON

            var qry = "";
            if (pid.length > 0)
                qry = "/" + pid;
            qry += "?query=" + pbody;
            syncJsonPost(root + qry, "GET", ""); // QueryString
            
        }

        function noteupdate() {
            var root = document.getElementById("txtpath").value;
            var prs = {
                "id": document.getElementById("txtid3").value,
                "body": document.getElementById("txtbody3").value
            };
            syncJsonPost(root, "PUT", JSON.stringify(prs))
        }

        function notedelete() {
            var root = document.getElementById("txtpath").value;
            var pid=document.getElementById("txtid4").value;
            if (pid.length > 0)
                qry = "/" + pid;
            syncJsonPost(root + qry, "DELETE", "");
        }

        function notecreate() {
            var root = document.getElementById("txtpath").value;
            var bod = document.getElementById("txtbody1").value;
            var prs = {
                "id": 0,
                "body": bod
            };
            syncJsonPost(root, "POST", JSON.stringify(prs));
        }

        </script>
        </form>
    </body>
    </html>