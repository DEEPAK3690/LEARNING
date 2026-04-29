import { useState } from "react";

function ProfileCard({
  id,
  name,
  role,
  bio,
  followers: initialFollowers,
  isFollowing: initialIsFollowing,
  image,
  onDelete,
}) {
  const [followers, setFollowers] = useState(initialFollowers);
  const [isFollowing, setIsFollowing] = useState(initialIsFollowing);

  function handleFollowClick() {
    const nextFollowing = !isFollowing;
    setIsFollowing(nextFollowing);
    setFollowers((current) => current + (nextFollowing ? 1 : -1));
  }

  function handleDeleteClick() {
    onDelete(id);
  }

  return (
    <div className="profile-card">
      <img src={image} alt={name} />

      <h1>{name}</h1>
      <h2>{role}</h2>

      <p>{bio}</p>

      <p>
        <strong>{followers}</strong> followers
      </p>

      <button onClick={handleFollowClick}>
        {isFollowing ? "Connected" : "Connect"}
      </button>

      <button className="delete-button" onClick={handleDeleteClick}>
        Delete
      </button>
    </div>
  );
}

export default ProfileCard;
