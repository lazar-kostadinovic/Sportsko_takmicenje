function getParametarIzUrla(parametar) {
    const urlSearchParams = new URLSearchParams(window.location.search);
    return urlSearchParams.get(parametar);
  }

  const takmicenjeId = getParametarIzUrla("takmicenjeId");
  console.log(takmicenjeId);

  async function prikaziLeaderboardTakmicenja() {
    try {
        const response = await fetch(
            `http://localhost:5064/api/LeaderboardTakmicenje/${takmicenjeId}`
        );

        if (!response.ok) {
            throw new Error(
                `Error fetching leaderboard for takmicenje ${takmicenjeId}`
            );
        }

        const data = await response.json();

        const leaderboardTable = document.getElementById("leaderboardTable");
        const tbody = leaderboardTable.querySelector("tbody");
        tbody.innerHTML = ""; 

        for (const entry of data) {
            const nazivTima = await getTimById(entry.timId);

            const newRow = document.createElement("tr");

            const nazivTimaCell = document.createElement("td");
            nazivTimaCell.textContent = nazivTima;
            newRow.appendChild(nazivTimaCell);
        
            const pobedeCell = document.createElement("td");
            pobedeCell.textContent = entry.pobede;
            newRow.appendChild(pobedeCell);

            tbody.appendChild(newRow);
        }
    } catch (error) {
        console.error(error);
    }
}


  async function azurirajLeaderboard() {
    const utakmicaIdsResponse = await fetch(
      `http://localhost:5064/api/Takmicenje/get-utakmiceID/${takmicenjeId}`
    );
    const utakmicaIds = await utakmicaIdsResponse.json();

    for (const utakmicaId of utakmicaIds) {
      const timIdsResponse = await fetch(
        `http://localhost:5064/api/Utakmica/teamIds/${utakmicaId}`
      );
      const timIds = await timIdsResponse.json();

      for (const timId of timIds) {
        await fetch(
          `http://localhost:5064/api/LeaderboardUtakmica/${utakmicaId}/${timId}`,
          {
            method: "POST",
          }
        );
      }

      const increaseWinsResponse = await fetch(
        `http://localhost:5064/api/LeaderboardTakmicenje/${utakmicaId}`,
        {
          method: "POST",
        }
      );
      const success = await increaseWinsResponse.json();
      console.log(`Success for match ${utakmicaId}: ${success}`);
    }
    console.log("Leaderboard update complete");
  }

  async function getTimById(id) {
    const response = await fetch(`http://localhost:5064/api/Tim/get-tim/${id}`);
    if (response.ok) {
      const timInfo = await response.json();

      return timInfo.naziv || null;
    } else {
      console.error(`Failed to get Tim by ID ${id}.`);
      return null;
    }
  }

  azurirajLeaderboard();
  prikaziLeaderboardTakmicenja();
