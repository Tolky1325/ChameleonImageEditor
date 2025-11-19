// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("leaderboardSearch");
    const resultsContainer = document.querySelector(".user_stats_all");
    let debounceTimer;

    if (searchInput) {
        searchInput.addEventListener("input", function (e) {
            const query = e.target.value;

            clearTimeout(debounceTimer);
            
            debounceTimer = setTimeout(() => {
                fetchLeaderboardData(query);
            }, 1000);
        });
    }

    function fetchLeaderboardData(query) {
        const urlParams = new URLSearchParams(window.location.search);
        const sort = urlParams.get('sort') || 'exported';
        const direction = urlParams.get('direction') || 'desc';

        const url = `/Leaderboard/Search?query=${encodeURIComponent(query)}&sort=${sort}&direction=${direction}`;

        fetch(url)
            .then(response => response.json())
            .then(data => {
                renderLeaderboard(data);
            })
            .catch(error => console.error('Error fetching leaderboard:', error));
    }

    function renderLeaderboard(users) {
        resultsContainer.innerHTML = ""; 

        if (users.length === 0) {
            resultsContainer.innerHTML = `
                <p style="text-align: center; padding: 20px;">
                    No users found.
                </p>`;
            return;
        }

        users.forEach(user => {
            
            const isFirst = user.rank === 1;
            const isTopThree = user.rank <= 3;

            const rowClass = isFirst ? "ranking-row rank-one" : "ranking-row";
            const rankColClass = isTopThree ? "rank-col top-rank" : "rank-col";

            let crownHtml = "";
            if (isFirst) {
                crownHtml = '<span class="material-symbols-outlined crown-icon">crown</span>';
            }

            const html = `
                <div class="${rowClass}">
                    <div class="${rankColClass}">
                        <span class="rank-number">${user.rank}</span>
                        ${crownHtml}
                    </div>

                    <div class="user-icon_container">
                        <span class="material-symbols-outlined user-icon">person</span>
                    </div>

                    <div class="nickname-col">
                        <span class="nickname-text">${user.username}</span>
                    </div>

                    <div class="stats-col">
                        <div class="iports_stat-value">
                            <span class="stat-value">${user.imports}</span>
                        </div>

                        <div class="edits_stat-value">
                            <span class="stat-value">${user.edits}</span>
                        </div>

                        <div class="exports_stat-value">
                            <span class="stat-value">${user.exported}</span>
                        </div>
                    </div>
                </div>
            `;

            resultsContainer.insertAdjacentHTML('beforeend', html);
        });
    }
});