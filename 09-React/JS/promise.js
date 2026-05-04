// ============================================================
//  JAVASCRIPT PROMISES — Step-by-Step Guide
// ============================================================

// ─────────────────────────────────────────────────────────────
// STEP 1: What is a Promise?
// ─────────────────────────────────────────────────────────────
// A Promise is an object that represents the eventual completion
// (or failure) of an asynchronous operation and its resulting value.
//
// A Promise is always in one of 3 states:
//   • pending   → initial state, neither fulfilled nor rejected
//   • fulfilled → operation completed successfully
//   • rejected  → operation failed


// ─────────────────────────────────────────────────────────────
// STEP 2: Creating a Promise
// ─────────────────────────────────────────────────────────────
// new Promise((resolve, reject) => { ... })
//   resolve(value)  → moves state from pending → fulfilled
//   reject(reason)  → moves state from pending → rejected

// const myFirstPromise = new Promise((resolve, reject) => {
//   const success = true; // simulate success or failure

//   if (success) {
//     resolve("Data fetched successfully!"); // fulfilled
//   } else {
//     reject("Something went wrong!");       // rejected
//   }
// });


// // ─────────────────────────────────────────────────────────────
// // STEP 3: Consuming a Promise — .then() and .catch()
// // ─────────────────────────────────────────────────────────────
// // .then(onFulfilled)  → runs when promise is resolved
// // .catch(onRejected)  → runs when promise is rejected
// // .finally()          → runs regardless of outcome

// myFirstPromise
//   .then((result) => {
//     console.log("✅ Success:", result);   // "Data fetched successfully!"
//   })
//   .catch((error) => {
//     console.log("❌ Error:", error);
//   })
//   .finally(() => {
//     console.log("🏁 Promise settled (done).");
//   });


// // ─────────────────────────────────────────────────────────────
// // STEP 4: Real-World Example — Simulating an API call
// // ─────────────────────────────────────────────────────────────

function fetchUserData(userId) {
  return new Promise((resolve, reject) => {
    setTimeout(() => {                    // simulate network delay
      if (userId === 1) {
        resolve({ id: 1, name: "Alice", age: 28 });
      } else {
        reject(new Error("User not found"));
      }
    }, 1500); // 1.5 second delay
  });
}

// fetchUserData(1)
//   .then((user) => console.log("User:", user))
//   .catch((err) => console.log("Error:", err.message));

// fetchUserData(99)
//   .then((user) => console.log("User:", user))
//   .catch((err) => console.log("Error:", err.message)); // "User not found"


// ─────────────────────────────────────────────────────────────
// STEP 5: Promise Chaining
// ─────────────────────────────────────────────────────────────
// Each .then() returns a NEW promise, allowing chaining.
// The return value of one .then() is passed to the next.

fetchUserData(1)
  .then((user) => {
    console.log("Step 1 — Got user:", user.name);
    return user.id * 10; // pass transformed value to next .then()
  })
  .then((transformedId) => {
    console.log("Step 2 — Transformed ID:", transformedId); // 10
    return "Chain complete!";
  })
  .then((message) => {
    console.log("Step 3 —", message);
  })
  .catch((err) => console.log("Chain error:", err.message));


// // ─────────────────────────────────────────────────────────────
// // STEP 6: Promise.all() — Run multiple promises in parallel
// // ─────────────────────────────────────────────────────────────
// // Waits for ALL promises to resolve.
// // Rejects immediately if ANY one promise rejects.

// const p1 = Promise.resolve("Result 1");
// const p2 = new Promise((resolve) => setTimeout(() => resolve("Result 2"), 500));
// const p3 = Promise.resolve("Result 3");

// Promise.all([p1, p2, p3])
//   .then((results) => console.log("All results:", results))
//   // ["Result 1", "Result 2", "Result 3"]
//   .catch((err) => console.log("One failed:", err));


// // ─────────────────────────────────────────────────────────────
// // STEP 7: Promise.allSettled() — Run all, get every outcome
// // ─────────────────────────────────────────────────────────────
// // Unlike Promise.all(), this NEVER rejects.
// // Returns an array of { status, value } or { status, reason }.

// const p4 = Promise.resolve("OK");
// const p5 = Promise.reject("Failed");
// const p6 = Promise.resolve("Also OK");

// Promise.allSettled([p4, p5, p6]).then((results) => {
//   results.forEach((r) => console.log(r));
//   // { status: 'fulfilled', value: 'OK' }
//   // { status: 'rejected',  reason: 'Failed' }
//   // { status: 'fulfilled', value: 'Also OK' }
// });


// // ─────────────────────────────────────────────────────────────
// // STEP 8: Promise.race() — First settled wins
// // ─────────────────────────────────────────────────────────────
// // Resolves/rejects as soon as the FIRST promise settles.

// const fast = new Promise((resolve) => setTimeout(() => resolve("Fast!"), 100));
// const slow = new Promise((resolve) => setTimeout(() => resolve("Slow!"), 500));

// Promise.race([fast, slow])
//   .then((winner) => console.log("Winner:", winner)); // "Fast!"


// // ─────────────────────────────────────────────────────────────
// // STEP 9: Promise.any() — First fulfilled wins
// // ─────────────────────────────────────────────────────────────
// // Like race(), but ignores rejections.
// // Only rejects if ALL promises reject.

// const fail1 = Promise.reject("Error A");
// const fail2 = Promise.reject("Error B");
// const success = Promise.resolve("First success!");

// Promise.any([fail1, fail2, success])
//   .then((val) => console.log("Any:", val)) // "First success!"
//   .catch((err) => console.log("All failed:", err));


// // ─────────────────────────────────────────────────────────────
// // STEP 10: async / await — Syntactic sugar over Promises
// // ─────────────────────────────────────────────────────────────
// // async function always returns a Promise.
// // await pauses execution until the Promise settles.

// async function getUser() {
//   try {
//     const user = await fetchUserData(1);    // waits here
//     console.log("async/await user:", user);

//     const missingUser = await fetchUserData(99); // will throw
//     console.log(missingUser);
//   } catch (err) {
//     console.log("async/await error:", err.message); // "User not found"
//   }
// }

// getUser();


// ─────────────────────────────────────────────────────────────
// SUMMARY
// ─────────────────────────────────────────────────────────────
//  new Promise(resolve, reject) → create
//  .then()                      → handle success
//  .catch()                     → handle error
//  .finally()                   → always runs
//  Promise.all()                → all must succeed
//  Promise.allSettled()         → get all outcomes
//  Promise.race()               → first settled wins
//  Promise.any()                → first fulfilled wins
//  async / await                → cleaner syntax for promises
// ─────────────────────────────────────────────────────────────
