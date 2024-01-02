var submitBtn = document.querySelector("#submit");
var urlInput = document.querySelector("#urlshort");
var getUrls = document.querySelector("#getUrls")

submitBtn.addEventListener("click", function (event) {
    let url = urlInput.value;
    console.log('in onClick');
    fetch("https://localhost:7242/", {
        method: "POST",
        body: JSON.stringify(url), // Send the URL as a plain string
        headers: {
            'Content-Type': 'application/json'
        }
    }).then(response => response.json()).then(response => { console.log({ response }) });
});


// Function to fetch and display URLs
getUrls.addEventListener("click", function (event) {
    fetch("/all")
        .then(response => response.json())
        .then(data => {
            // Get the element where you want to display the URLs
            const urlListElement = document.getElementById("urlList");

            // Create a table to display the URLs
            const table = document.createElement("table");
            table.border = "1"; // Add a border to the table (optional)

            // Create table header
            const headerRow = table.insertRow();
            const headerCell1 = headerRow.insertCell(0);
            headerCell1.textContent = "Shortened Link Name | ";
            const headerCell2 = headerRow.insertCell(1);
            headerCell2.textContent = "Link Address  |";
            const headerCell3 = headerRow.insertCell(2);
            headerCell3.textContent = "Time Created |";
            const headerCell4 = headerRow.insertCell(3);
            headerCell4.textContent = "Original Url";

            // Iterate through the retrieved URLs and create table rows
            data.forEach(urlInfo => {
                const row = table.insertRow();
                const cell1 = row.insertCell(0);
                cell1.textContent = urlInfo.token; // Shortened Link Name

                const cell2 = row.insertCell(1);
                const a = document.createElement("a");
                a.textContent = urlInfo.shortenedURL; // Link Address
                a.href = urlInfo.shortenedURL;
                a.target = "_blank"; // Open links in a new tab/window
                cell2.appendChild(a);

                const cell3 = row.insertCell(2);
                const createdDate = new Date(urlInfo.created);
                cell3.textContent = createdDate.toLocaleString(); // Time Created

                const cell4 = row.insertCell(3);
                const a2 = document.createElement("a");
                a2.textContent = urlInfo.url;
                a2.href = urlInfo.url;
                a2.target = "_blank"; // Open links in a new tab/window
                cell4.appendChild(a2);
            });

            // Append the table to the element
            urlListElement.appendChild(table);
        })
        .catch(error => {
            console.error("Error fetching URLs:", error);
        });
});