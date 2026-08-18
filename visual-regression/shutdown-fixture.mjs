export default async function shutdownFixture() {
  try {
    await fetch('http://127.0.0.1:4207/__shutdown', { method: 'POST' });
  } catch (error) {
    if (error?.cause?.code !== 'ECONNREFUSED') throw error;
  }
}
