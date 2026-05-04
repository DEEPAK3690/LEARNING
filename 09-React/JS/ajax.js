/**
 * AJAX (Asynchronous JavaScript and XML) - Detailed Explanation
 * 
 * AJAX allows web pages to send and receive data from a server asynchronously
 * without reloading the entire page.
 */

// ============================================================================
// STEP 1: WHAT IS AJAX?
// ============================================================================
/*
 * AJAX is a technique that combines:
 * - JavaScript: For handling client-side logic
 * - XMLHttpRequest: For communicating with server asynchronously
 * - DOM: For dynamically updating page content
 * - Server-side scripts: For processing requests
 *
 * Key Benefit: Asynchronous communication means the page remains responsive
 * while waiting for server responses.
 */

// ============================================================================
// STEP 2: XMLHttpRequest OBJECT (Traditional AJAX)
// ============================================================================
// Note: XMLHttpRequest only works in browsers. This file is educational.
// To run: use in an HTML file in a browser, or use the Fetch API examples below.

// Creating an XMLHttpRequest object (browser only)
// const xhr = new XMLHttpRequest();

// ============================================================================
// STEP 3: AJAX REQUEST LIFECYCLE (5 steps)
// ============================================================================

/*
 * STEP 1: Create XMLHttpRequest object
 * STEP 2: Define what happens when response is received (onreadystatechange)
 * STEP 3: Open connection (method, URL, async)
 * STEP 4: Send the request
 * STEP 5: Handle response data
 */

// Full Example:
function ajaxExample() {
  // STEP 1: Create XMLHttpRequest
  const xhr = new XMLHttpRequest();

  // STEP 2: Set up event listener for state changes
  xhr.onreadystatechange = function() {
    // readyState values:
    // 0 = UNSENT (request not initialized)
    // 1 = OPENED (request opened)
    // 2 = HEADERS_RECEIVED (response headers received)
    // 3 = LOADING (response body loading)
    // 4 = DONE (request complete)

    if (xhr.readyState === 4) {
      // STEP 5: Handle response
      if (xhr.status === 200) {
        // Success (HTTP 200 OK)
        const response = JSON.parse(xhr.responseText);
        console.log("Data received:", response);
      } else {
        console.error("Error:", xhr.status);
      }
    }
  };

  // STEP 3: Open connection
  // xhr.open(method, url, async);
  xhr.open("GET", "https://api.example.com/data", true);

  // STEP 4: Send request
  xhr.send();
}

// ============================================================================
// STEP 4: HTTP METHODS IN AJAX
// ============================================================================

// GET Request - Retrieve data
function getRequest() {
  const xhr = new XMLHttpRequest();
  xhr.onreadystatechange = function() {
    if (xhr.readyState === 4 && xhr.status === 200) {
      console.log(xhr.responseText);
    }
  };
  xhr.open("GET", "https://api.example.com/users", true);
  xhr.send();
}

// POST Request - Send data to server
function postRequest() {
  const xhr = new XMLHttpRequest();
  xhr.onreadystatechange = function() {
    if (xhr.readyState === 4 && xhr.status === 201) {
      console.log("Data sent successfully");
    }
  };
  xhr.open("POST", "https://api.example.com/users", true);
  xhr.setRequestHeader("Content-Type", "application/json");
  
  const data = JSON.stringify({ name: "John", age: 30 });
  xhr.send(data);
}

// PUT Request - Update existing data
function putRequest() {
  const xhr = new XMLHttpRequest();
  xhr.open("PUT", "https://api.example.com/users/1", true);
  xhr.setRequestHeader("Content-Type", "application/json");
  xhr.send(JSON.stringify({ name: "Jane", age: 25 }));
}

// DELETE Request - Remove data
function deleteRequest() {
  const xhr = new XMLHttpRequest();
  xhr.open("DELETE", "https://api.example.com/users/1", true);
  xhr.send();
}

// ============================================================================
// STEP 5: AJAX WITH ERROR HANDLING
// ============================================================================

function ajaxWithErrorHandling() {
  const xhr = new XMLHttpRequest();

  xhr.onreadystatechange = function() {
    if (xhr.readyState === 4) {
      if (xhr.status === 200) {
        console.log("Success:", xhr.responseText);
      } else if (xhr.status === 404) {
        console.error("Not Found");
      } else if (xhr.status === 500) {
        console.error("Server Error");
      }
    }
  };

  xhr.onerror = function() {
    console.error("Network error occurred");
  };

  xhr.open("GET", "https://api.example.com/data", true);
  xhr.send();
}

// ============================================================================
// STEP 6: MODERN AJAX - FETCH API (Recommended)
// ============================================================================

// Fetch is simpler and returns Promises
function fetchExample() {
  fetch("https://api.example.com/data")
    .then(response => response.json())
    .then(data => console.log(data))
    .catch(error => console.error("Error:", error));
}

// Fetch with POST
function fetchPost() {
  fetch("https://api.example.com/users", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ name: "John", age: 30 })
  })
    .then(response => response.json())
    .then(data => console.log(data))
    .catch(error => console.error(error));
}

// ============================================================================
// STEP 7: ASYNC/AWAIT (Modern Alternative)
// ============================================================================

async function fetchWithAsyncAwait() {
  try {
    const response = await fetch("https://api.example.com/data");
    const data = await response.json();
    console.log(data);
  } catch (error) {
    console.error("Error:", error);
  }
}

// ============================================================================
// STEP 8: KEY AJAX CONCEPTS
// ============================================================================

/*
 * 1. ASYNCHRONOUS: Request doesn't block user interaction
 * 2. PARTIAL PAGE UPDATE: Only specific parts of page update
 * 3. BACKGROUND COMMUNICATION: Data exchange happens silently
 * 4. REDUCED BANDWIDTH: Only necessary data is transferred
 * 5. IMPROVED USER EXPERIENCE: Smooth, responsive interactions
 */

// ============================================================================
// STEP 9: AJAX RESPONSE TYPES
// ============================================================================

function handleDifferentResponseTypes() {
  const xhr = new XMLHttpRequest();

  xhr.onreadystatechange = function() {
    if (xhr.readyState === 4 && xhr.status === 200) {
      // Text response
      const text = xhr.responseText;

      // JSON response
      const json = JSON.parse(xhr.responseText);

      // XML response
      const xml = xhr.responseXML;

      // Array Buffer (binary)
      const arrayBuffer = xhr.response;
    }
  };

  xhr.open("GET", "https://api.example.com/data", true);
  xhr.send();
}

// ============================================================================
// STEP 10: PRACTICAL AJAX WORKFLOW
// ============================================================================

/*
 * 1. User performs action (click button, submit form, etc.)
 * 2. JavaScript captures event
 * 3. AJAX request sent to server
 * 4. Server processes request and sends response
 * 5. Response data received by JavaScript
 * 6. DOM updated dynamically with new data
 * 7. User sees updated content without page reload
 */

// Example workflow
function loadUserData(userId) {
  const xhr = new XMLHttpRequest();

  // Show loading indicator
  document.getElementById("loader").style.display = "block";

  xhr.onreadystatechange = function() {
    if (xhr.readyState === 4) {
      // Hide loading indicator
      document.getElementById("loader").style.display = "none";

      if (xhr.status === 200) {
        const userData = JSON.parse(xhr.responseText);
        // Update DOM with user data
        document.getElementById("userName").textContent = userData.name;
        document.getElementById("userEmail").textContent = userData.email;
      }
    }
  };

  xhr.open("GET", `https://api.example.com/users/${userId}`, true);
  xhr.send();
}
