var submitBtn = document.querySelector('#submit');
var urlInput = document.querySelector('#urlshort');
var getUrlBtn = document.querySelector('#getUrls');
var desiredToken = document.querySelector('#desiredToken');
// add error message
var respArea = document.querySelector('#resp-area');
var isMessageAdded = false;
var messageElement = document.createElement('p');

messageElement.textContent =
  'Your desired token should be at least 4 characters, creating a randomized short URL.';

function originalUrl(url) {
  fetch('/', {
    method: 'POST',
    body: JSON.stringify(url), // Send the URL as a plain string
    headers: {
      'Content-Type': 'application/json'
    }
  })
    .then(response => response.json())
    .then(response => {
      console.log({ response });
    });

  getUrls();
  clearInput();
}

let isTablePopulated = false;
// Function to fetch and display URLs

function getUrls() {
  fetch('/all')
    .then(response => response.json())
    .then(data => {
      // Get the element where you want to display the URLs
      const urlListElement = document.getElementById('urlList');
      if (isTablePopulated) {
        // Clear the existing content of the element
        urlListElement.innerHTML = '';
      }

      // Create a table to display the URLs
      const table = document.createElement('table');
      table.border = '1'; // Add a border to the table (optional)

      // Create table header
      const headerRow = table.insertRow();
      const headerCell1 = headerRow.insertCell(0);
      headerCell1.textContent = 'Short Link Name';
      const headerCell2 = headerRow.insertCell(1);
      headerCell2.textContent = 'Link Address  |';
      const headerCell3 = headerRow.insertCell(2);
      headerCell3.textContent = 'Original Url |';
      const headerCell4 = headerRow.insertCell(3);
      headerCell4.textContent = 'Date Created';

      // Iterate through the retrieved URLs and create table rows
      data.forEach(urlInfo => {
        const row = table.insertRow();
        const cell1 = row.insertCell(0);
        cell1.textContent = urlInfo.token; // Shortened Link Name

        const cell2 = row.insertCell(1);
        const a = document.createElement('a');
        a.className =
          'link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover';
        a.textContent = urlInfo.shortenedURL; // Link Address
        a.href = urlInfo.shortenedURL;
        a.target = '_blank'; // Open links in a new tab/window
        cell2.appendChild(a);

        const cell3 = row.insertCell(2);
        const a2 = document.createElement('a');
        a2.className =
          'link-secondary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover';
        let truncateUrl = urlInfo.url.substring(
          urlInfo.url.indexOf('://') + 3,
          40
        );
        if (urlInfo.url.length > 50) {
          truncateUrl = truncateUrl + '...';
        }
        a2.textContent = truncateUrl;
        a2.href = urlInfo.url;
        a2.target = '_blank'; // Open links in a new tab/window
        cell3.appendChild(a2);

        const cell4 = row.insertCell(3);
        const createdDate = new Date(urlInfo.created);

        // Format date in European style (24-hour clock, no AM/PM)
        const options = {
          year: 'numeric',
          month: 'numeric',
          day: 'numeric',
          // hour: 'numeric',
          // minute: 'numeric',
          // second: 'numeric',
          hour12: false
        };
        cell4.textContent = createdDate.toLocaleString('en-GB', options); // Time Created
      });
      isTablePopulated = true;
      // Append the table to the element
      urlListElement.appendChild(table);
    })
    .catch(error => {
      console.error('Error fetching URLs:', error);
    });
}

function customUrl(url, token) {
  console.log(url, token);

  if (url != null && token != null) {
    console.log('continue with custom url');
    // Send the data to the backend using an HTTP POST request
    fetch('/shorten', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ url: url, token: token })
    })
      .then(response => response.json())
      .then(data => {
        console.log(data);
      });
    clearInput();
  } else {
    console.log('in else so exiting...');
    return;
  }
}

// get urls
getUrlBtn.addEventListener('click', function () {
  getUrls();
});

// do url magic
submitBtn.addEventListener('click', function () {
  let url = urlInput.value;
  let desiredtoken = desiredToken.value;
  try {
    // create a shortened url with random token
    if (url != null && desiredtoken.length <= 4) {
      console.error('your desired token should be atleast 4 chars');
      respArea.appendChild(messageElement);
      isMessageAdded = true;
      originalUrl(url);
    } else if (url != null && desiredtoken != null) {
      // create url with custom token
      if (isMessageAdded) {
        respArea.removeChild(messageElement);
        isMessageAdded = false;
      }
      customUrl(url, desiredtoken);
    }
    // either way get urls after completion
    setTimeOut(() => {
      getUrls();
    }, 1500);
  } catch (error) {
    console.error('somethings wrong in funkytown ->', error);
  }
});

function clearInput() {
  // let 1 sec pass before clearing input
  setTimeout(function () {
    urlInput.value = '';
    desiredToken.value = '';
  }, 1000);
}
// nice non blocking way to get urls
requestAnimationFrame(() => getUrls());
