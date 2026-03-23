export default function LoginView({
  username,
  password,
  setUsername,
  setPassword,
  login,
  error,
}) {
  return (
    <main className="page">
      <section className="panel login-panel">
        <header className="panel-header">
          <p className="eyebrow">SmartQueue Login</p>
          <h1>JWT Access</h1>
          <p className="subtitle">
            Demo users: admin/admin123, staff/staff123, customer/customer123.
          </p>
        </header>

        <form className="login-form" onSubmit={login}>
          <label>
            Username
            <input
              placeholder="Enter your username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
            />
          </label>
          <label>
            Password
            <input
              type="password"
              placeholder="Enter your password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
            />
          </label>
          <button className="primary login-submit" type="submit">
            Sign in
          </button>
        </form>
        {error && <p className="error">{error}</p>}
      </section>
    </main>
  );
}
