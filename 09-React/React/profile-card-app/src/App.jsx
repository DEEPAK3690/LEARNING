import { useEffect, useState } from "react";
import "./App.css";
import ProfileCard from "./ProfileCard";

const fallbackProfiles = [
  {
    id: 1,
    name: "Aarav",
    role: "Beginner React Developer",
    bio: "Learning React by building small real projects.",
    followers: 120,
    isFollowing: false,
    image: "https://images.unsplash.com/photo-1494790108377-be9c29b29330",
  },
  {
    id: 2,
    name: "Mia",
    role: "Frontend Learner",
    bio: "Practicing components, props, and state every day.",
    followers: 87,
    isFollowing: false,
    image: "https://images.unsplash.com/photo-1438761681033-6461ffad8d80",
  },
  {
    id: 3,
    name: "Noah",
    role: "UI Explorer",
    bio: "Turning simple ideas into working React apps.",
    followers: 154,
    isFollowing: false,
    image: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e",
  },
];

function App() {
  const [profiles, setProfiles] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [name, setName] = useState("");
  const [role, setRole] = useState("");
  const [bio, setBio] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    let ignore = false;

    async function loadProfiles() {
      try {
        setIsLoading(true);
        setError("");

        const response = await fetch("https://dummyjson.com/users?limit=6");

        if (!response.ok) {
          throw new Error("Failed to load profiles");
        }

        const data = await response.json();

        const loadedProfiles = data.users.map((user) => ({
          id: user.id,
          name: `${user.firstName} ${user.lastName}`.trim(),
          role: user.company?.title || "Frontend Developer",
          bio: user.university
            ? `Studies at ${user.university}.`
            : "Learning React step by step.",
          followers: 100 + user.id,
          isFollowing: false,
          image: user.image,
        }));

        if (!ignore) {
          setProfiles(loadedProfiles);
        }
      } catch {
        if (!ignore) {
          setError("Could not reach the API, so sample profiles are showing.");
          setProfiles(fallbackProfiles);
        }
      } finally {
        if (!ignore) {
          setIsLoading(false);
        }
      }
    }

    loadProfiles();

    return () => {
      ignore = true;
    };
  }, []);

  const filteredProfiles = profiles.filter((profile) =>
    profile.name.toLowerCase().includes(searchText.toLowerCase())
  );

  function handleSubmit(event) {
    event.preventDefault();

    if (!name.trim() || !role.trim() || !bio.trim()) return;

    const newProfile = {
      id: Date.now(),
      name: name.trim(),
      role: role.trim(),
      bio: bio.trim(),
      followers: 0,
      isFollowing: false,
      image: "https://images.unsplash.com/photo-1494790108377-be9c29b29330",
    };

    setProfiles((current) => [newProfile, ...current]);
    setName("");
    setRole("");
    setBio("");
  }

  function handleDeleteProfile(id) {
    setProfiles((current) => current.filter((profile) => profile.id !== id));
  }

  const showEmptyState = !isLoading && filteredProfiles.length === 0;

  return (
    <div className="app">
      <div className="page-header">
        <h1>Profile Cards</h1>
        <input
          type="text"
          placeholder="Search profiles..."
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          disabled={isLoading}
        />
      </div>

      <form className="profile-form" onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          disabled={isLoading}
        />
        <input
          type="text"
          placeholder="Role"
          value={role}
          onChange={(e) => setRole(e.target.value)}
          disabled={isLoading}
        />
        <textarea
          placeholder="Bio"
          rows="3"
          value={bio}
          onChange={(e) => setBio(e.target.value)}
          disabled={isLoading}
        />
        <button type="submit" disabled={isLoading}>
          Add Profile
        </button>
      </form>

      {isLoading ? <p className="status-message">Loading profiles...</p> : null}

      {error ? <p className="status-message error-message">{error}</p> : null}

      {showEmptyState ? (
        <p className="empty-state">No profiles found.</p>
      ) : null}

      {!isLoading && filteredProfiles.length > 0 ? (
        <div className="card-grid">
          {filteredProfiles.map((profile) => (
            <ProfileCard
              key={profile.id}
              {...profile}
              onDelete={handleDeleteProfile}
            />
          ))}
        </div>
      ) : null}
    </div>
  );
}

export default App;
